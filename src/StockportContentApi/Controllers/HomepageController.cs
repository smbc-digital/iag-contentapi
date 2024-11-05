namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = false)]
public class HomepageController : Controller
{
    private readonly Func<string, HomepageRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public HomepageController(ResponseHandler handler,
        Func<string, HomepageRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/homepage")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() =>
        {
            HomepageRepository repository = _createRepository(businessId);
            return repository.Get();
        });
}