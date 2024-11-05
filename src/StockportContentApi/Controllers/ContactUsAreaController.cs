namespace StockportContentApi.Controllers;

public class ContactUsController : Controller
{
    private readonly Func<string, ContactUsAreaRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ContactUsController(ResponseHandler handler,
        Func<string, ContactUsAreaRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/contactusarea")]
    public async Task<IActionResult> GetContactUsArea(string businessId) =>
        await _handler.Get(() =>
        {
            ContactUsAreaRepository repository = _createRepository(businessId);
            Task<HttpResponse> contactUsArea = repository.GetContactUsArea();

            return contactUsArea;
        });
}