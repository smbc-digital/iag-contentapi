namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ServicePayPaymentController : Controller
{
    private readonly Func<string, IServicePayPaymentRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ServicePayPaymentController(ResponseHandler handler,
        Func<string, IServicePayPaymentRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/service-pay-payments/{slug}")]
    [Route("v1/{businessId}/service-pay-payments/{slug}")]
    public async Task<IActionResult> GetPayment(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            IServicePayPaymentRepository paymentRepository = _createRepository(businessId);

            return paymentRepository.GetPayment(slug);
        });
}