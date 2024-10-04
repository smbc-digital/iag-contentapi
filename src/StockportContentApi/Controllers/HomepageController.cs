namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = false)]
public class HomepageController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, HomepageRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public HomepageController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, HomepageRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/homepage")]
    [Route("v1/{businessId}/homepage")]
    public async Task<IActionResult> Get(string businessId)
    {
        IActionResult result = await _handler.Get(() =>
        {
            HomepageRepository repository = _createRepository(_createConfig(businessId));
            return repository.Get();
        });

        return result;
    }
}