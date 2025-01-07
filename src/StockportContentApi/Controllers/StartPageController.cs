namespace StockportContentApi.Controllers;

public class StartPageController(ResponseHandler handler,
                                Func<string, StartPageRepository> createRepository) : Controller
{
    private readonly Func<string, StartPageRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/start-page/{slug}")]
    [Route("v1/{businessId}/start-page/{slug}")]
    public async Task<IActionResult> GetStartPage(string slug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetStartPage(slug));

    [HttpGet]
    [Route("{businessId}/start-page/")]
    [Route("v1/{businessId}/start-page/")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() => _createRepository(businessId).Get());
}