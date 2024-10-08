namespace StockportContentApi.Controllers;

public class SiteHeaderController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, SiteHeaderRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public SiteHeaderController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, SiteHeaderRepository> createRepository) 
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/header")]
    [Route("v1/{businessId}/header")]
    public async Task <IActionResult> Index(string businessId) =>
        await _handler.Get(() =>
        {
            SiteHeaderRepository siteHeaderRepository = _createRepository(_createConfig(businessId));
            return siteHeaderRepository.GetSiteHeader();
        });
}
