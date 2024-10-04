namespace StockportContentApi.Controllers;

public class ContactUsController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, ContactUsAreaRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ContactUsController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, ContactUsAreaRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/contactusarea")]
    [Route("v1/{businessId}/contactusarea")]
    public async Task<IActionResult> GetContactUsArea(string contactUsSlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            ContactUsAreaRepository repository = _createRepository(_createConfig(businessId));
            Task<HttpResponse> contactUsArea = repository.GetContactUsArea();

            return contactUsArea;
        });
    }
}