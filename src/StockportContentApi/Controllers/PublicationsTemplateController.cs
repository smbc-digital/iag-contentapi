namespace StockportContentApi.Controllers;

public class PublicationsTemplateController(ResponseHandler handler,
                                Func<string, IPublicationsTemplateRepository> createRepository) : Controller
{
    private readonly Func<string, IPublicationsTemplateRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/publications/{slug}")]
    [Route("v1/{businessId}/publications/{slug}")]
    public async Task<IActionResult> GetPublicationsTemplate(string slug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetPublicationsTemplate(slug, businessId));
}