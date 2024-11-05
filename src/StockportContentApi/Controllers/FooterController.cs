namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class FooterController : Controller
{
    private readonly Func<string, FooterRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public FooterController(ResponseHandler handler,
        Func<string, FooterRepository> createRepository)
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
            FooterRepository footerRepository = _createRepository(businessId);
            return footerRepository.GetFooter();
        });
}