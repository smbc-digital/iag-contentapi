namespace StockportContentApi.Controllers;

public class SectionController : Controller
{
    private readonly Func<string, SectionRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public SectionController(ResponseHandler handler,
        Func<string, SectionRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/sections/{sectionSlug}")]
    public async Task<IActionResult> GetSection(string sectionSlug, string businessId) =>
        await _handler.Get(() =>
        {
            SectionRepository repository = _createRepository(businessId);
            Task<HttpResponse> section = repository.GetSections(sectionSlug);

            return section;
        });

    [HttpGet]
    [Route("v1/{businessId}/sectionsitemap")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() =>
        {
            SectionRepository repository = _createRepository(businessId);
            Task<HttpResponse> section = repository.Get();

            return section;
        });
}