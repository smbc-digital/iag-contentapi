namespace StockportContentApi.Controllers;

public class SiteHeaderController(ResponseHandler handler,
                                Func<string, ISiteHeaderRepository> createRepository) : Controller
{
    private readonly Func<string, ISiteHeaderRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/header")]
    [Route("v1/{businessId}/header")]
    public async Task <IActionResult> Index(string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetSiteHeader());
}
