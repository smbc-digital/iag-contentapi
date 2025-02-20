﻿namespace StockportContentApi.Repositories;

public interface IRedirectsRepository
{
    Task<HttpResponse> GetRedirects();
    Task<HttpResponse> GetUpdatedRedirects();
}

public class RedirectsRepository(IContentfulClientManager clientManager,
                                Func<string, ContentfulConfig> createConfig,
                                RedirectBusinessIds redirectBusinessIds,
                                IContentfulFactory<ContentfulRedirect,
                                BusinessIdToRedirects> contenfulFactory,
                                ShortUrlRedirects shortUrlRedirects,
                                LegacyUrlRedirects legacyUrlRedirects) : BaseRepository
{
    public IContentfulClientManager ClientManager = clientManager;
    private const string ContentType = "redirect";
    private readonly Func<string, ContentfulConfig> _createConfig = createConfig;
    private readonly RedirectBusinessIds _redirectBusinessIds = redirectBusinessIds;
    private IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects> _contenfulFactory = contenfulFactory;
    private readonly ShortUrlRedirects _shortUrlRedirects = shortUrlRedirects;
    private readonly LegacyUrlRedirects _legacyUrlRedirects = legacyUrlRedirects;

    public async Task<HttpResponse> GetRedirects()
    {
        if (_legacyUrlRedirects.Redirects.Count > 0 || _shortUrlRedirects.Redirects.Count > 0)
            return HttpResponse.Successful(new Redirects(_shortUrlRedirects.Redirects, _legacyUrlRedirects.Redirects));

        return await GetUpdatedRedirects();
    }

    public async Task<HttpResponse> GetUpdatedRedirects()
    {
        Dictionary<string, BusinessIdToRedirects> redirectPerBusinessId = new();

        foreach (string businessId in _redirectBusinessIds.BusinessIds)
            redirectPerBusinessId.Add(businessId, await GetRedirectForBusinessId(businessId));

        return !redirectPerBusinessId.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "Redirects not found")
            : HttpResponse.Successful(GetRedirectsFromBusinessIdToRedirectsDictionary(redirectPerBusinessId));
    }

    private Redirects GetRedirectsFromBusinessIdToRedirectsDictionary(Dictionary<string, BusinessIdToRedirects> redirects)
    {
        Dictionary<string, RedirectDictionary> shortUrlRedirects = new();
        Dictionary<string, RedirectDictionary> legacyUrlRedirects = new();

        foreach (string businessId in redirects.Keys)
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
        ContentfulConfig config = _createConfig(businessId);

        _client = ClientManager.GetClient(config);
        QueryBuilder<ContentfulRedirect> builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs(ContentType).Include(1);
        ContentfulCollection<ContentfulRedirect> entries = await _client.GetEntries(builder);

        return !entries.Any()
            ? new NullBusinessIdToRedirects()
            : _contenfulFactory.ToModel(entries.FirstOrDefault());
    }
}