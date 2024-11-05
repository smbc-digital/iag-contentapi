namespace StockportContentApi.Controllers;

public class LandingPageController : Controller
{
    private readonly Func<string, LandingPageRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public LandingPageController(ResponseHandler handler,
        Func<string, LandingPageRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/landing/{slug}")]
    public async Task<IActionResult> GetLandingPage(string slug, string businessId) =>
        await _handler.Get(() =>
        {
            LandingPageRepository repository = _createRepository(businessId);
            Task<HttpResponse> landingPage = repository.GetLandingPage(slug);

            return landingPage;
        });
}