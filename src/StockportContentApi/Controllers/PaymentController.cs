namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class PaymentController : Controller
{
    private readonly Func<string, IPaymentRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public PaymentController(ResponseHandler handler,
        Func<string, IPaymentRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/payments/{slug}")]
    [Route("v1/{businessId}/payments/{slug}")]
    public async Task<IActionResult> GetPayment(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            IPaymentRepository paymentRepository = _createRepository(businessId);

            return paymentRepository.GetPayment(slug);
        });

    [HttpGet]
    [Route("{businessId}/payments")]
    [Route("v1/{businessId}/payments")]
    public async Task<IActionResult> Index(string businessId) =>
        await _handler.Get(() =>
        {
            IPaymentRepository paymentRepository = _createRepository(businessId);

            return paymentRepository.Get();
        });
}