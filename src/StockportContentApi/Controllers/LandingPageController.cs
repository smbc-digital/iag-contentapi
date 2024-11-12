namespace StockportContentApi.Controllers;

public class LandingPageController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<string, CacheKeyConfig> _cacheKeyConfig;
    private readonly Func<ContentfulConfig, CacheKeyConfig, LandingPageRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public LandingPageController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<string, CacheKeyConfig> cacheKeyConfig,
        Func<ContentfulConfig, CacheKeyConfig, LandingPageRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _cacheKeyConfig = cacheKeyConfig;   
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/landing/{slug}")]
    [Route("v1/{businessId}/landing/{slug}")]
    public async Task<IActionResult> GetLandingPage(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            LandingPageRepository repository = _createRepository(_createConfig(businessId), _cacheKeyConfig(businessId));
            Task<HttpResponse> landingPage = repository.GetLandingPage(slug);

            return landingPage;
        });
}