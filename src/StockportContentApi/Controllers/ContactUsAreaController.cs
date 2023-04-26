namespace StockportContentApi.Controllers;

public class ContactUsController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, ContactUsAreaRepository> _createRepository;

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
            var repository = _createRepository(_createConfig(businessId));
            var contactUsArea = repository.GetContactUsArea();

            return contactUsArea;
        });
    }
}