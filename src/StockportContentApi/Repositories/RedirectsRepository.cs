namespace StockportContentApi.Repositories;

public class RedirectsRepository : BaseRepository
{
    public IContentfulClientManager ClientManager;
    private const string ContentType = "redirect";
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly RedirectBusinessIds _redirectBusinessIds;
    private Contentful.Core.IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects> _contenfulFactory;
    private readonly ShortUrlRedirects _shortUrlRedirects;
    private readonly LegacyUrlRedirects _legacyUrlRedirects;

    public RedirectsRepository(IContentfulClientManager clientManager,
            Func<string, ContentfulConfig> createConfig,
            RedirectBusinessIds redirectBusinessIds,
            IContentfulFactory<ContentfulRedirect,
            BusinessIdToRedirects> contenfulFactory,
            ShortUrlRedirects shortUrlRedirects,
            LegacyUrlRedirects legacyUrlRedirects)
    {
        _createConfig = createConfig;
        _redirectBusinessIds = redirectBusinessIds;
        ClientManager = clientManager;
        _contenfulFactory = contenfulFactory;
        _shortUrlRedirects = shortUrlRedirects;
        _legacyUrlRedirects = legacyUrlRedirects;
    }

    public async Task<HttpResponse> GetRedirects()
    {
        if (_legacyUrlRedirects.Redirects.Count > 0 || _shortUrlRedirects.Redirects.Count > 0)
            return HttpResponse.Successful(new Redirects(_shortUrlRedirects.Redirects, _legacyUrlRedirects.Redirects));

        return await GetUpdatedRedirects();
    }

    public async Task<HttpResponse> GetUpdatedRedirects()
    {
        var redirectPerBusinessId = new Dictionary<string, BusinessIdToRedirects>();

        foreach (var businessId in _redirectBusinessIds.BusinessIds)
        {
            redirectPerBusinessId.Add(businessId, await GetRedirectForBusinessId(businessId));
        }

        return !redirectPerBusinessId.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "Redirects not found")
            : HttpResponse.Successful(GetRedirectsFromBusinessIdToRedirectsDictionary(redirectPerBusinessId));
    }

    private Redirects GetRedirectsFromBusinessIdToRedirectsDictionary(Dictionary<string, BusinessIdToRedirects> redirects)
    {
        var shortUrlRedirects = new Dictionary<string, RedirectDictionary>();
        var legacyUrlRedirects = new Dictionary<string, RedirectDictionary>();

        foreach (var businessId in redirects.Keys)
        {
            shortUrlRedirects.Add(businessId, redirects[businessId].ShortUrlRedirects);
            legacyUrlRedirects.Add(businessId, redirects[businessId].LegacyUrlRedirects);
        }

        _shortUrlRedirects.Redirects = shortUrlRedirects;
        _legacyUrlRedirects.Redirects = legacyUrlRedirects;

        return new Redirects(_shortUrlRedirects.Redirects, _legacyUrlRedirects.Redirects);
    }

    private async Task<BusinessIdToRedirects> GetRedirectForBusinessId(string businessId)
    {
        var config = _createConfig(businessId);

        _client = ClientManager.GetClient(config);
        var builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs(ContentType).Include(1);
        var entries = await _client.GetEntries(builder);

        return !entries.Any()
            ? new NullBusinessIdToRedirects()
            : _contenfulFactory.ToModel(entries.FirstOrDefault());
    }
}