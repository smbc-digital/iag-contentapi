namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ContactUsIdController : Controller
{
    private readonly Func<string, ContactUsIdRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ContactUsIdController(ResponseHandler handler,
        Func<string, ContactUsIdRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/contact-us-id/{slug}")]
    [Route("v1/{businessId}/contact-us-id/{slug}")]
    public async Task<IActionResult> Detail(string slug, string businessId)
        => await _handler.Get(() =>
        {
            ContactUsIdRepository contactUsIdRepository = _createRepository(businessId);
            return contactUsIdRepository.GetContactUsIds(slug);
        });
}