namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = false)]
public class HomepageController(ResponseHandler handler,
                                Func<string, HomepageRepository> createRepository) : Controller
{
    private readonly Func<string, HomepageRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/homepage")]
    [Route("v1/{businessId}/homepage")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() => _createRepository(businessId).Get());
}