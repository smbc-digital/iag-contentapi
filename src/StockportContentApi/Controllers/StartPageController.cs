namespace StockportContentApi.Controllers;

public class StartPageController : Controller
{
    private readonly Func<string, StartPageRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public StartPageController(ResponseHandler handler,
        Func<string, StartPageRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/start-page/{slug}")]
    public async Task<IActionResult> GetStartPage(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            StartPageRepository startPageRepository = _createRepository(businessId);
            return startPageRepository.GetStartPage(slug);
        });

    [HttpGet]
    [Route("v1/{businessId}/start-page/")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() =>
        {
            StartPageRepository startRepository = _createRepository(businessId);
            return startRepository.Get();
        });
}