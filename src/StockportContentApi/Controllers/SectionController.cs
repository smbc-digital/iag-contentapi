namespace StockportContentApi.Controllers;

public class SectionController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, SectionRepository> _createRepository;

    public SectionController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, SectionRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/sections/{sectionSlug}")]
    [Route("v1/{businessId}/sections/{sectionSlug}")]
    public async Task<IActionResult> GetSection(string sectionSlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            var repository = _createRepository(_createConfig(businessId));
            var section = repository.GetSections(sectionSlug);

            return section;
        });
    }

    [HttpGet]
    [Route("{businessId}/sectionsitemap")]
    [Route("v1/{businessId}/sectionsitemap")]
    public async Task<IActionResult> Get(string businessId)
    {
        return await _handler.Get(() =>
        {
            var repository = _createRepository(_createConfig(businessId));
            var section = repository.Get();

            return section;
        });
    }
}