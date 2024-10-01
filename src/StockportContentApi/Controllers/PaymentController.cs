namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class PaymentController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, PaymentRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public PaymentController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, PaymentRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/payments/{slug}")]
    [Route("v1/{businessId}/payments/{slug}")]
    public async Task<IActionResult> GetPayment(string slug, string businessId)
    {
        return await _handler.Get(() =>
        {
            PaymentRepository paymentRepository = _createRepository(_createConfig(businessId));
            return paymentRepository.GetPayment(slug);
        });
    }

    [HttpGet]
    [Route("{businessId}/payments")]
    [Route("v1/{businessId}/payments")]
    public async Task<IActionResult> Index(string businessId)
    {
        return await _handler.Get(() =>
        {
            PaymentRepository paymentRepository = _createRepository(_createConfig(businessId));
            return paymentRepository.Get();
        });
    }
}