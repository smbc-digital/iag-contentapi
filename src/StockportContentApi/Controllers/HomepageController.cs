namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = false)]
public class HomepageController : Controller
{
    private readonly Func<string, IHomepageRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public HomepageController(ResponseHandler handler,
        Func<string, IHomepageRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/homepage")]
    [Route("v1/{businessId}/homepage")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() =>
        {
            IHomepageRepository repository = _createRepository(businessId);
            return repository.Get();
        });
}