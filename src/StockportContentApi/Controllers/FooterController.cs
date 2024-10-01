namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class FooterController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, FooterRepository> _createRepository;

    public FooterController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, FooterRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/footer")]
    [Route("v1/{businessId}/footer")]
    public async Task<IActionResult> GetFooter(string businessId)
    {
        IActionResult response = await _handler.Get(() =>
        {
            FooterRepository footerRepository = _createRepository(_createConfig(businessId));
            return footerRepository.GetFooter();
        });

        return response;
    }
}