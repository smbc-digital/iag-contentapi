namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ServicePayPaymentController : Controller
{
    private readonly Func<string, ServicePayPaymentRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ServicePayPaymentController(ResponseHandler handler,
        Func<string, ServicePayPaymentRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/service-pay-payments/{slug}")]
    public async Task<IActionResult> GetPayment(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            ServicePayPaymentRepository paymentRepository = _createRepository(businessId);
            return paymentRepository.GetPayment(slug);
        });
}