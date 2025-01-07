namespace StockportContentApi.Controllers;

public class SectionController(ResponseHandler handler,
                            Func<string, SectionRepository> createRepository) : Controller
{
    private readonly Func<string, SectionRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/sections/{sectionSlug}")]
    [Route("v1/{businessId}/sections/{sectionSlug}")]
    public async Task<IActionResult> GetSection(string sectionSlug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetSections(sectionSlug));

    [HttpGet]
    [Route("{businessId}/sectionsitemap")]
    [Route("v1/{businessId}/sectionsitemap")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() => _createRepository(businessId).Get());
}