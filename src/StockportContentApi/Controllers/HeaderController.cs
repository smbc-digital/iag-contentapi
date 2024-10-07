namespace StockportContentApi.Controllers;

public class HeaderController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, HeaderRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public HeaderController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, HeaderRepository> createRepository) 
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/header")]
    [Route("v1/{businessId}/header")]
    public async Task <IActionResult> Index(string businessId)
    {
        IActionResult response = await _handler.Get(() =>
        {
            HeaderRepository headerRepository = _createRepository(_createConfig(businessId));
            return headerRepository.GetHeader();
        });

        return response;
    }
}
