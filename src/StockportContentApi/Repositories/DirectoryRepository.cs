using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("StockportContentApiTests")] 
namespace StockportContentApi.Repositories;

public interface IDirectoryRepository
{
    Task<HttpResponse> Get();
    Task<HttpResponse> Get(string slug);
    Task<HttpResponse> GetEntry(string slug);
}

public class DirectoryRepository : BaseRepository, IDirectoryRepository
{
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulDirectory, Directory> _directoryFactory;
    private readonly IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> _directoryEntryFactory;
    private readonly ICache _cache;
    private readonly ILogger<DirectoryRepository> _logger;
    private readonly RedisExpiryConfiguration _redisExpiryConfiguration;
    
    // TODO Move this to config!!
    private  int DepthLimit { get; } = 5;
    
    public DirectoryRepository(ContentfulConfig config, 
        IContentfulClientManager clientManager,
        IContentfulFactory<ContentfulDirectory, Directory> directoryFactory,
        IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> directoryEntryFactory,
        ICache cache,
        IOptions<RedisExpiryConfiguration> redisExpiryConfiguration,
        ILogger<DirectoryRepository> logger)
    {
        _client = clientManager.GetClient(config);
        _directoryFactory = directoryFactory;
        _directoryEntryFactory = directoryEntryFactory;
        _cache = cache;
        _redisExpiryConfiguration = redisExpiryConfiguration.Value;
        _logger = logger;
    }

    public async Task<HttpResponse> Get(string slug)
    {
        Directory directory = await _cache.GetFromCacheOrDirectlyAsync(slug, () => GetDirectoryFromSource(slug, 0), _redisExpiryConfiguration.Directory);

        return directory is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Directory not found {slug}")
            : HttpResponse.Successful(directory);
    }

    public async Task<HttpResponse> Get()
    {
        QueryBuilder<ContentfulDirectory> builder = new QueryBuilder<ContentfulDirectory>().ContentTypeIs("directory").Include(1);
        ContentfulCollection<ContentfulDirectory> entries = await GetAllEntriesAsync(_client, builder);

        if (entries == null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No entries found");

        IEnumerable<ContentfulDirectory> contentfulDirectories = entries as IEnumerable<ContentfulDirectory> ?? entries.ToList();
        List<Directory> directoriesList = contentfulDirectories.Select(directory => _directoryFactory.ToModel(directory)).ToList();
        
        foreach (Directory directory in directoriesList)
        {
            directory.Entries = (await GetAllDirectoryEntries()).Where(directoryEntry => directoryEntry.Directories is not null &&
                directoryEntry.Directories.Any(dir => dir.Slug == directory.Slug));
        }

        return !directoriesList.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No directories found")
            : HttpResponse.Successful(directoriesList);
    }

    public async Task<HttpResponse> GetEntry(string slug)
    {
        IEnumerable<DirectoryEntry> directoryEntries = await GetAllDirectoryEntries();
        DirectoryEntry directoryEntry = directoryEntries?.SingleOrDefault(directoryEntry => directoryEntry.Slug == slug);

        return directoryEntry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Directory entry not found {slug}")
            : HttpResponse.Successful(directoryEntry);  
    }

    internal async Task<Directory> GetDirectoryFromSource(string slug, int depth)
    {
        if (depth > DepthLimit)
            return null;

        QueryBuilder<ContentfulDirectory> builder = new QueryBuilder<ContentfulDirectory>().ContentTypeIs("directory").FieldEquals("fields.slug", slug).Include(2);
        ContentfulCollection<ContentfulDirectory> contentfulDirectories = await GetAllEntriesAsync(_client, builder);

        if (!contentfulDirectories.Any())
            return null;

        ContentfulDirectory contentfulDirectory = contentfulDirectories.SingleOrDefault();
        Directory directory = _directoryFactory.ToModel(contentfulDirectory);
        directory.Entries = await GetDirectoryEntriesForDirectory(directory.Slug);

        Task<Directory>[] contentfulSubDirectoriesTasks = contentfulDirectory
            .SubDirectories?.Select(async subDirectory => 
                await _cache.GetFromCacheOrDirectlyAsync(subDirectory.Slug, () => GetDirectoryFromSource(subDirectory.Slug, depth + 1), _redisExpiryConfiguration.Directory)).ToArray();

        if (contentfulSubDirectoriesTasks is not null)
        {
            Task.WaitAll(contentfulSubDirectoriesTasks);
            directory.SubDirectories = contentfulSubDirectoriesTasks.Select(tsk => tsk.Result);
        }
            
        return directory;
    }

    internal async Task<IEnumerable<DirectoryEntry>> GetAllDirectoryEntries() =>
        await _cache.GetFromCacheOrDirectlyAsync("directory-entries-all", () => GetAllDirectoryEntriesFromSource(), _redisExpiryConfiguration.Directory);

    internal async Task<IEnumerable<DirectoryEntry>> GetDirectoryEntriesForDirectory(string slug) => 
        (await GetAllDirectoryEntries())
            .Where(directoryEntry => directoryEntry.Directories is not null
                && directoryEntry.Directories.Any(directory => directory.Slug == slug));
    
    internal async Task<IEnumerable<DirectoryEntry>> GetAllDirectoryEntriesFromSource()
    {
        QueryBuilder<ContentfulDirectoryEntry> builder = new QueryBuilder<ContentfulDirectoryEntry>().ContentTypeIs("group").Include(1);
        ContentfulCollection<ContentfulDirectoryEntry> entries = await GetAllEntriesAsync(_client, builder);
        IEnumerable<ContentfulDirectoryEntry> contentfulDirectoryEntries = entries as IEnumerable<ContentfulDirectoryEntry> ?? entries.ToList();
        return contentfulDirectoryEntries.Select(g => _directoryEntryFactory.ToModel(g));
    }
}