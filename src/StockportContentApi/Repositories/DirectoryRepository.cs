using Directory = StockportContentApi.Model.Directory;

namespace StockportContentApi.Repositories;

public interface IDirectorypRepository
{
    Task<HttpResponse> Get(string slug);
}

public class DirectoryRepository : BaseRepository, IDirectorypRepository
{
    private readonly Contentful.Core.IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulDirectory, Directory> _directoryFactory;

    public DirectoryRepository(ContentfulConfig config, 
                                IContentfulClientManager clientManager,
                                IContentfulFactory<ContentfulDirectory, Directory> directoryFactory)
    {
        _client = clientManager.GetClient(config);
        _directoryFactory = directoryFactory;
    }

    public async Task<HttpResponse> Get(string slug)
    {
        var builder = new QueryBuilder<ContentfulDirectory>().ContentTypeIs("directory").FieldEquals("fields.slug", slug).Include(1);
        var entries = await GetAllEntriesAsync(_client, builder);
        var contentfulDirectories = entries as IEnumerable<ContentfulDirectory> ?? entries.ToList();

        var directories = contentfulDirectories.Select(g => _directoryFactory.ToModel(g));

        return entries == null || !directories.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Directory not found {slug}")
            : HttpResponse.Successful(directories.ToList());
    }
}
