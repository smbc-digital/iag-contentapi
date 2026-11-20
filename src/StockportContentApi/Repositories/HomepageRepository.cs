namespace StockportContentApi.Repositories;

public interface IHomepageRepository
{
    Task<HttpResponse> Get(string tagId);
}

public class HomepageRepository(ContentfulConfig config,
                                IContentfulClientManager clientManager,
                                IContentfulFactory<ContentfulHomepage, Homepage> homepageFactory,
                                ICache cache) : IHomepageRepository
{
    private const int CacheDurationMinutes = 10;

    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulHomepage, Homepage> _homepageFactory = homepageFactory;
    private readonly ICache _cache = cache;

    public async Task<HttpResponse> Get(string tagId)
    {
        ContentfulHomepage entry = await _cache.GetFromCacheOrDirectlyWithLockingAsync(
            $"{config.BusinessId}-homepage-{tagId}",
            () => GetHomepageEntry(tagId),
            CacheDurationMinutes,
            "Contentful");

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No homepage found")
            : HttpResponse.Successful(_homepageFactory.ToModel(entry));
    }

    private async Task<ContentfulHomepage> GetHomepageEntry(string tagId)
    {
        QueryBuilder<ContentfulHomepage> builder = new QueryBuilder<ContentfulHomepage>()
            .ContentTypeIs("homepage")
            .FieldExists("metadata.tags")
            .FieldEquals("metadata.tags.sys.id[in]", tagId)
            .Include(2);
        
        ContentfulCollection<ContentfulHomepage> entries = await _client.GetEntries(builder);
        return entries.FirstOrDefault();
    }
}