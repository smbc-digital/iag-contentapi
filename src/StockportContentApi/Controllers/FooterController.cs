namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class FooterController(ResponseHandler handler,
                            Func<string, IFooterRepository> createRepository) : Controller
{
    private readonly Func<string, IFooterRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/footer")]
    [Route("v1/{businessId}/footer")]
    public async Task<IActionResult> GetFooter(string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetFooter());
}