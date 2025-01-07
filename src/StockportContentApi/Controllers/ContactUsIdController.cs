namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ContactUsIdController(ResponseHandler handler,
                                Func<string, ContactUsIdRepository> createRepository) : Controller
{
    private readonly Func<string, ContactUsIdRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/contact-us-id/{slug}")]
    [Route("v1/{businessId}/contact-us-id/{slug}")]
    public async Task<IActionResult> Detail(string slug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetContactUsIds(slug));
}