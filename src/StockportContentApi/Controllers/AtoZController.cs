namespace StockportContentApi.Controllers;


public class AtoZController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, AtoZRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public AtoZController(ResponseHandler handler,
    Func<string, ContentfulConfig> createConfig,
    Func<ContentfulConfig, AtoZRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/atoz/")]
    [Route("v1/{businessId}/atoz/")]
    public async Task<IActionResult> Index(string businessId)
        => await _handler.Get(()
            => _createRepository(_createConfig(businessId)).Get());

    [HttpGet]
    [Route("{businessId}/atoz/{letter}")]
    [Route("v1/{businessId}/atoz/{letter}")]
    public async Task<IActionResult> Index(string letter, string businessId)
        => await _handler.Get(() 
            => _createRepository(_createConfig(businessId)).Get(letter));
}