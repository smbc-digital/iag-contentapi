using Directory = StockportContentApi.Model.Directory;

namespace StockportContentApi.Repositories;

public interface IDirectoryEntryRepository
{
    Task<HttpResponse> Get(string slug);
    Task<HttpResponse> GetEntriesInDirectory(string id);
}

public class DirectoryEntryRepository : BaseRepository, IDirectoryEntryRepository
{
    private readonly Contentful.Core.IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> _directoryEntryFactory;

    public DirectoryEntryRepository(ContentfulConfig config, 
                                IContentfulClientManager clientManager,
                                IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> directoryEntryFactory
        )
    {
        _client = clientManager.GetClient(config);
        _directoryEntryFactory = directoryEntryFactory;
    }

    public async Task<HttpResponse> Get(string slug)
    {
        var builder = new QueryBuilder<ContentfulDirectoryEntry>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(2);
        var entries = await GetAllEntriesAsync(_client, builder);
        var contentfulDirectoryEntries = entries as IEnumerable<ContentfulDirectoryEntry> ?? entries.ToList();

        var directoryEntry = contentfulDirectoryEntries.Select(g => _directoryEntryFactory.ToModel(g));

        return entries == null || !directoryEntry.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Directory entry not found {slug}")
            : HttpResponse.Successful(directoryEntry.ToList());
    }

    public async Task<HttpResponse> GetEntriesInDirectory(string id)
    {
        var builder = new QueryBuilder<ContentfulDirectoryEntry>().ContentTypeIs("group").LinksToEntry(id).Include(2);
        var entries = await GetAllEntriesAsync(_client, builder);
        var contentfulDirectoryEntries = entries as IEnumerable<ContentfulDirectoryEntry> ?? entries.ToList();

        var directoryEntry = contentfulDirectoryEntries.Select(g => _directoryEntryFactory.ToModel(g));

        return entries == null || !directoryEntry.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Directory not found {id}")
            : HttpResponse.Successful(directoryEntry.ToList());
    }
}
