namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ServicePayPaymentController(ResponseHandler handler,
                                        Func<string, IServicePayPaymentRepository> createRepository) : Controller
{
    private readonly Func<string, IServicePayPaymentRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/service-pay-payments/{slug}")]
    [Route("v1/{businessId}/service-pay-payments/{slug}")]
    public async Task<IActionResult> GetPayment(string slug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetPayment(slug));
}