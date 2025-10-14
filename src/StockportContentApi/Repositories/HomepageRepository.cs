namespace StockportContentApi.Repositories;

public interface IHomepageRepository
{
    Task<HttpResponse> Get(string tagId);
}

public class HomepageRepository(ContentfulConfig config,
                                IContentfulClientManager clientManager,
                                IContentfulFactory<ContentfulHomepage, Homepage> homepageFactory) : IHomepageRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulHomepage, Homepage> _homepageFactory = homepageFactory;

    public async Task<HttpResponse> Get(string tagId)
    {
        QueryBuilder<ContentfulHomepage> builder = new QueryBuilder<ContentfulHomepage>()
            .ContentTypeIs("homepage")
            .FieldExists("metadata.tags")
            .FieldEquals("metadata.tags.sys.id[in]", tagId)
            .Include(2);
        
        ContentfulCollection<ContentfulHomepage> entries = await _client.GetEntries(builder);
        ContentfulHomepage entry = entries.FirstOrDefault();

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No homepage found")
            : HttpResponse.Successful(_homepageFactory.ToModel(entry));
    }
}