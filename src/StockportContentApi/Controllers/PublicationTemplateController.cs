namespace StockportContentApi.Controllers;

public class PublicationTemplateController(ResponseHandler handler,
                                Func<string, IPublicationTemplateRepository> createRepository) : Controller
{
    private readonly Func<string, IPublicationTemplateRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/publications/{slug}")]
    [Route("v1/{businessId}/publications/{slug}")]
    public async Task<IActionResult> GetPublicationTemplate(string slug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetPublicationTemplate(slug, businessId));
}