namespace StockportContentApi.Controllers;

public class ShowcaseController : Controller
{
    private readonly Func<string, ShowcaseRepository> _createRepository;

    private readonly ResponseHandler _handler;

    public ShowcaseController(ResponseHandler handler,
        Func<string, ShowcaseRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/showcases/{showcaseSlug}")]
    public async Task<IActionResult> GetShowcase(string showcaseSlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            ShowcaseRepository repository = _createRepository(businessId);
            Task<HttpResponse> showcase = repository.GetShowcases(showcaseSlug);

            return showcase;
        });
    }

    [HttpGet]
    [Route("v1/{businessId}/showcases/")]
    public async Task<IActionResult> Get(string businessId)
    {
        return await _handler.Get(() =>
        {
            ShowcaseRepository repository = _createRepository(businessId);
            Task<HttpResponse> showcase = repository.Get();

            return showcase;
        });
    }
}