namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class PaymentController : Controller
{
    private readonly Func<string, PaymentRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public PaymentController(ResponseHandler handler,
        Func<string, PaymentRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/payments/{slug}")]
    public async Task<IActionResult> GetPayment(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            PaymentRepository paymentRepository = _createRepository(businessId);

            return paymentRepository.GetPayment(slug);
        });

    [HttpGet]
    [Route("v1/{businessId}/payments")]
    public async Task<IActionResult> Index(string businessId) =>
        await _handler.Get(() =>
        {
            PaymentRepository paymentRepository = _createRepository(businessId);

            return paymentRepository.Get();
        });
}