namespace StockportContentApi.Controllers;

public class ShowcaseController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<string, CacheKeyConfig> _cacheKeyConfig;
    private readonly Func<ContentfulConfig, CacheKeyConfig, ShowcaseRepository> _createRepository;

    private readonly ResponseHandler _handler;

    public ShowcaseController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<string, CacheKeyConfig> cacheKeyConfig,
        Func<ContentfulConfig, CacheKeyConfig, ShowcaseRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _cacheKeyConfig = cacheKeyConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/showcases/{showcaseSlug}")]
    [Route("v1/{businessId}/showcases/{showcaseSlug}")]
    public async Task<IActionResult> GetShowcase(string showcaseSlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            ShowcaseRepository repository = _createRepository(_createConfig(businessId), _cacheKeyConfig(businessId));
            Task<HttpResponse> showcase = repository.GetShowcases(showcaseSlug);

            return showcase;
        });
    }

    [HttpGet]
    [Route("{businessId}/showcases/")]
    [Route("v1/{businessId}/showcases/")]
    public async Task<IActionResult> Get(string businessId)
    {
        return await _handler.Get(() =>
        {
            ShowcaseRepository repository = _createRepository(_createConfig(businessId), _cacheKeyConfig(businessId));
            Task<HttpResponse> showcase = repository.Get();

            return showcase;
        });
    }
}