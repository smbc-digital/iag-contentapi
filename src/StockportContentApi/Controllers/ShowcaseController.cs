namespace StockportContentApi.Controllers;

public class ShowcaseController : Controller
{

    private readonly ResponseHandler _handler;
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, ShowcaseRepository> _createRepository;

    public ShowcaseController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, ShowcaseRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/showcases/{showcaseSlug}")]
    [Route("v1/{businessId}/showcases/{showcaseSlug}")]
    public async Task<IActionResult> GetShowcase(string showcaseSlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            ShowcaseRepository repository = _createRepository(_createConfig(businessId));
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
            ShowcaseRepository repository = _createRepository(_createConfig(businessId));
            Task<HttpResponse> showcase = repository.Get();

            return showcase;
        });
    }
}