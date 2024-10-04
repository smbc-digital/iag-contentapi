namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ServicePayPaymentController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, ServicePayPaymentRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ServicePayPaymentController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, ServicePayPaymentRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/service-pay-payments/{slug}")]
    [Route("v1/{businessId}/service-pay-payments/{slug}")]
    public async Task<IActionResult> GetPayment(string slug, string businessId)
    {
        return await _handler.Get(() =>
        {
            ServicePayPaymentRepository paymentRepository = _createRepository(_createConfig(businessId));

            return paymentRepository.GetPayment(slug);
        });
    }
}