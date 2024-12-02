namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class FooterController : Controller
{
    private readonly Func<string, IFooterRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public FooterController(ResponseHandler handler,
        Func<string, IFooterRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/footer")]
    [Route("v1/{businessId}/footer")]
    public async Task<IActionResult> GetFooter(string businessId) =>
        await _handler.Get(() =>
        {
            IFooterRepository footerRepository = _createRepository(businessId);
            return footerRepository.GetFooter();
        });
}