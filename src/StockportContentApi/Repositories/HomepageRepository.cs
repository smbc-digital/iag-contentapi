namespace StockportContentApi.Repositories;

public interface IHomepageRepository
{
    Task<HttpResponse> Get(string tagId);
}

public class HomepageRepository(ContentfulConfig config,
                                IContentfulClientManager clientManager,
                                IHomepageContentfulFactory homepageFactory,
                                ICache cache) : IHomepageRepository
{
    private const int CacheDurationMinutes = 10;

    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IHomepageContentfulFactory _homepageFactory = homepageFactory;
    private readonly ICache _cache = cache;

    public async Task<HttpResponse> Get(string tagId)
    {
        Homepage homepage = await _cache.GetFromCacheOrDirectlyWithLockingAsync(
            $"{config.BusinessId}-homepage-mapped-{tagId}",
            () => GetHomepageFromSource(tagId),
            CacheDurationMinutes,
            "Contentful");

        return homepage is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No homepage found")
            : HttpResponse.Successful(_homepageFactory.FilterForCurrentDate(homepage));
    }

    private async Task<Homepage> GetHomepageFromSource(string tagId)
    {
        QueryBuilder<ContentfulHomepage> builder = new QueryBuilder<ContentfulHomepage>()
            .ContentTypeIs("homepage")
            .FieldExists("metadata.tags")
            .FieldEquals("metadata.tags.sys.id[in]", tagId)
            .Include(2);
        
        ContentfulCollection<ContentfulHomepage> entries = await _client.GetEntries(builder);
        ContentfulHomepage entry = entries.FirstOrDefault();

        return entry is null
            ? null
            : _homepageFactory.ToCacheModel(entry);
    }
}