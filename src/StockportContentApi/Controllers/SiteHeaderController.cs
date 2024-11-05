namespace StockportContentApi.Controllers;

public class SiteHeaderController : Controller
{
    private readonly Func<string, SiteHeaderRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public SiteHeaderController(ResponseHandler handler,
        Func<string, SiteHeaderRepository> createRepository) 
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/header")]
    public async Task <IActionResult> Index(string businessId) =>
        await _handler.Get(() =>
        {
            SiteHeaderRepository siteHeaderRepository = _createRepository(businessId);
            return siteHeaderRepository.GetSiteHeader();
        });
}
