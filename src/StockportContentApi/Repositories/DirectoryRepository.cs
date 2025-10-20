using System.Runtime.CompilerServices;
using Amazon.SecretsManager.Model;
[assembly: InternalsVisibleTo("StockportContentApiTests")]
namespace StockportContentApi.Repositories;

public interface IDirectoryRepository
{
    Task<HttpResponse> Get(string tagId);
    Task<HttpResponse> Get(string slug, string tagId);
    Task<HttpResponse> GetEntry(string slug, string tagId);
}

public class DirectoryRepository(ContentfulConfig config,
                                IContentfulClientManager clientManager,
                                IContentfulFactory<ContentfulDirectory, Directory> directoryFactory,
                                IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> directoryEntryFactory,
                                ICache cache,
                                IOptions<RedisExpiryConfiguration> redisExpiryConfiguration) : BaseRepository, IDirectoryRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulDirectory, Directory> _directoryFactory = directoryFactory;
    private readonly IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> _directoryEntryFactory = directoryEntryFactory;
    private readonly ICache _cache = cache;
    private readonly RedisExpiryConfiguration _redisExpiryConfiguration = redisExpiryConfiguration.Value;

    // TODO Move this to config!!
    private int DepthLimit { get; } = 5;

    public async Task<HttpResponse> Get(string slug, string tagId)
    {
        Directory directory = await _cache.GetFromCacheOrDirectlyAsync(slug, () => GetDirectoryFromSource(slug, 0, tagId), _redisExpiryConfiguration.Directory);

        return directory is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Directory not found {slug}")
            : HttpResponse.Successful(directory);
    }

    public async Task<HttpResponse> Get(string tagId)
    {
        QueryBuilder<ContentfulDirectory> builder = new QueryBuilder<ContentfulDirectory>()
            .ContentTypeIs("directory")
            .Include(1);
        
        ContentfulCollection<ContentfulDirectory> entries = await GetAllEntriesAsync(_client, builder);

        if (entries is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No entries found");

        IEnumerable<ContentfulDirectory> contentfulDirectories = entries as IEnumerable<ContentfulDirectory> ?? entries.ToList();
        List<Directory> directoriesList = contentfulDirectories.Select(_directoryFactory.ToModel).ToList();

        foreach (Directory directory in directoriesList)
        {
            directory.Entries = (await GetAllDirectoryEntries(tagId))
                                    .Where(directoryEntry => directoryEntry.Directories is not null &&
                                        directoryEntry.Directories.Any(dir => dir.Slug.Equals(directory.Slug)));
        }

        return !directoriesList.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No directories found")
            : HttpResponse.Successful(directoriesList);
    }

    public async Task<HttpResponse> GetEntry(string slug, string tagId)
    {
        IEnumerable<DirectoryEntry> directoryEntries = await GetAllDirectoryEntries(tagId);
        DirectoryEntry directoryEntry = directoryEntries?.SingleOrDefault(directoryEntry => directoryEntry.Slug.Equals(slug));

        return directoryEntry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Directory entry not found {slug}")
            : HttpResponse.Successful(directoryEntry);
    }

    internal async Task<Directory> GetDirectoryFromSource(string slug, int depth, string tagId)
    {
        if (depth > DepthLimit)
            return null;

        QueryBuilder<ContentfulDirectory> builder = new QueryBuilder<ContentfulDirectory>()
            .ContentTypeIs("directory")
            .FieldEquals("fields.slug", slug)
            .Include(2);
        
        ContentfulCollection<ContentfulDirectory> contentfulDirectories = await GetAllEntriesAsync(_client, builder);

        if (!contentfulDirectories.Any())
            return null;

        ContentfulDirectory contentfulDirectory = contentfulDirectories.SingleOrDefault();
        Directory directory = _directoryFactory.ToModel(contentfulDirectory);
        directory.Entries = await GetDirectoryEntriesForDirectory(directory.Slug, tagId);

        Task<Directory>[] contentfulSubDirectoriesTasks = contentfulDirectory
            .SubDirectories?.Select(async subDirectory =>
                await _cache.GetFromCacheOrDirectlyAsync(subDirectory.Slug, () => GetDirectoryFromSource(subDirectory.Slug, depth + 1, tagId), _redisExpiryConfiguration.Directory)).ToArray();

        if (contentfulSubDirectoriesTasks is not null)
        {
            Task.WaitAll(contentfulSubDirectoriesTasks);
            directory.SubDirectories = contentfulSubDirectoriesTasks.Select(tsk => tsk.Result);
        }

        return directory;
    }

    internal async Task<IEnumerable<DirectoryEntry>> GetAllDirectoryEntries(string tagId) =>
        await _cache.GetFromCacheOrDirectlyAsync("directory-entries-all", () => GetAllDirectoryEntriesFromSource(tagId), _redisExpiryConfiguration.Directory);

    internal async Task<IEnumerable<DirectoryEntry>> GetDirectoryEntriesForDirectory(string slug, string tagId) =>
        (await GetAllDirectoryEntries(tagId))
            .Where(directoryEntry => directoryEntry.Directories is not null
                && directoryEntry.Directories.Any(directory => directory.Slug.Equals(slug)));

    internal async Task<IEnumerable<DirectoryEntry>> GetAllDirectoryEntriesFromSource(string tagId)
    {
        QueryBuilder<ContentfulDirectoryEntry> builder = new QueryBuilder<ContentfulDirectoryEntry>()
            .ContentTypeIs("group")
            .Include(1);
        ContentfulCollection<ContentfulDirectoryEntry> entries = await GetAllEntriesAsync(_client, builder);
        IEnumerable<ContentfulDirectoryEntry> contentfulDirectoryEntries = entries as IEnumerable<ContentfulDirectoryEntry> ?? entries.ToList();
        
        return contentfulDirectoryEntries.Select(_directoryEntryFactory.ToModel);
    }
}