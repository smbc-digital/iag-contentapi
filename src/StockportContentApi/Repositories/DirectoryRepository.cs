using Directory = StockportContentApi.Model.Directory;

namespace StockportContentApi.Repositories;

public interface IDirectoryRepository
{
    Task<HttpResponse> Get(string slug);
    Task<HttpResponse> GetEntry(string slug);
}

public class DirectoryRepository : BaseRepository, IDirectoryRepository
{
    private readonly Contentful.Core.IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulDirectory, Directory> _directoryFactory;
    private readonly IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> _directoryEntryFactory;

    public DirectoryRepository(ContentfulConfig config, 
                                IContentfulClientManager clientManager,
                                IContentfulFactory<ContentfulDirectory, Directory> directoryFactory,
                                IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> directoryEntryFactory)
    {
        _client = clientManager.GetClient(config);
        _directoryFactory = directoryFactory;
        _directoryEntryFactory = directoryEntryFactory;
    }

    public async Task<HttpResponse> Get(string slug)
    {
        var builder = new QueryBuilder<ContentfulDirectory>().ContentTypeIs("directory").FieldEquals("fields.slug", slug).Include(1);
        var entries = await GetAllEntriesAsync(_client, builder);
        var contentfulDirectories = entries as IEnumerable<ContentfulDirectory> ?? entries.ToList();

        var directory = contentfulDirectories.Select(g => _directoryFactory.ToModel(g)).SingleOrDefault();
        directory.Entries = await GetEntriesInDirectory(directory.ContentfulId);

        return entries is null || directory is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Directory not found {slug}")
            : HttpResponse.Successful(directory);
    }
    public async Task<HttpResponse> GetEntry(string slug)
    {
        var builder = new QueryBuilder<ContentfulDirectoryEntry>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(2);
        var entries = await GetAllEntriesAsync(_client, builder);
        var contentfulDirectoryEntries = entries as IEnumerable<ContentfulDirectoryEntry> ?? entries.ToList();

        var directoryEntry = contentfulDirectoryEntries.Select(g => _directoryEntryFactory.ToModel(g));

        return entries == null || !directoryEntry.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Directory entry not found {slug}")
            : HttpResponse.Successful(directoryEntry.ToList());
    }

    private async Task<IEnumerable<Model.DirectoryEntry>> GetEntriesInDirectory(string id)
    {
        var builder = new QueryBuilder<ContentfulDirectoryEntry>().ContentTypeIs("group").LinksToEntry(id).Include(2);
        var entries = await GetAllEntriesAsync(_client, builder);
        var contentfulDirectoryEntries = entries as IEnumerable<ContentfulDirectoryEntry> ?? entries.ToList();

        var directoryEntries = contentfulDirectoryEntries.Select(g => _directoryEntryFactory.ToModel(g));

        return directoryEntries;
    }
}
