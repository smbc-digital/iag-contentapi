namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class PaymentController(ResponseHandler handler,
                            Func<string, PaymentRepository> createRepository) : Controller
{
    private readonly Func<string, PaymentRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/payments/{slug}")]
    [Route("v1/{businessId}/payments/{slug}")]
    public async Task<IActionResult> GetPayment(string slug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetPayment(slug));

    [HttpGet]
    [Route("{businessId}/payments")]
    [Route("v1/{businessId}/payments")]
    public async Task<IActionResult> Index(string businessId) =>
        await _handler.Get(() => _createRepository(businessId).Get());
}