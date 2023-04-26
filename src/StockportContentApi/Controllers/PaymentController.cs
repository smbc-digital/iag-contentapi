namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class PaymentController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, PaymentRepository> _createRepository;

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
            var paymentRepository = _createRepository(_createConfig(businessId));
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
            var paymentRepository = _createRepository(_createConfig(businessId));
            return paymentRepository.Get();
        });
    }
}