namespace StockportContentApi.Controllers;

public class LandingPageController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, LandingPageRepository> _createRepository;

    private readonly ResponseHandler _handler;

    public LandingPageController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, LandingPageRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/landing/{slug}")]
    [Route("v1/{businessId}/landing/{slug}")]
    public async Task<IActionResult> GetLandingPage(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            LandingPageRepository repository = _createRepository(_createConfig(businessId));
            Task<HttpResponse> landingPage = repository.GetLandingPage(slug);

            return landingPage;
        });
}