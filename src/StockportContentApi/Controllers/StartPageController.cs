namespace StockportContentApi.Controllers;

public class StartPageController : Controller
{
    private readonly Func<string, IStartPageRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public StartPageController(ResponseHandler handler,
        Func<string, IStartPageRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/start-page/{slug}")]
    [Route("v1/{businessId}/start-page/{slug}")]
    public async Task<IActionResult> GetStartPage(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            IStartPageRepository startPageRepository = _createRepository(businessId);

            return startPageRepository.GetStartPage(slug);
        });

    [HttpGet]
    [Route("{businessId}/start-page/")]
    [Route("v1/{businessId}/start-page/")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() =>
        {
            IStartPageRepository startRepository = _createRepository(businessId);

            return startRepository.Get();
        });
}