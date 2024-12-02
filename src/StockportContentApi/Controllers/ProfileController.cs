namespace StockportContentApi.Controllers;

public class ProfileController : Controller
{
    private readonly Func<string, IProfileRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ProfileController(ResponseHandler handler,
        Func<string, IProfileRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/profiles/{profileSlug}")]
    [Route("v1/{businessId}/profiles/{profileSlug}")]
    public async Task<IActionResult> GetProfile(string profileSlug, string businessId) =>
        await _handler.Get(() =>
        {
            IProfileRepository repository = _createRepository(businessId);
            Task<HttpResponse> profile = repository.GetProfile(profileSlug);

            return profile;
        });

    [HttpGet]
    [Route("{businessId}/profiles/")]
    [Route("v1/{businessId}/profiles/")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() =>
        {
            IProfileRepository repository = _createRepository(businessId);
            Task<HttpResponse> profile = repository.Get();

            return profile;
        });
}