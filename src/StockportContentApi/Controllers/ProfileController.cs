namespace StockportContentApi.Controllers;

public class ProfileController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, IProfileRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public ProfileController(ResponseHandler handler, Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, IProfileRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/profiles/{profileSlug}")]
    [Route("v1/{businessId}/profiles/{profileSlug}")]
    public async Task<IActionResult> GetProfile(string profileSlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            IProfileRepository repository = _createRepository(_createConfig(businessId));
            Task<HttpResponse> profile = repository.GetProfile(profileSlug);

            return profile;
        });
    }

    [HttpGet]
    [Route("{businessId}/profiles/")]
    [Route("v1/{businessId}/profiles/")]
    public async Task<IActionResult> Get(string businessId)
    {
        return await _handler.Get(() =>
        {
            IProfileRepository repository = _createRepository(_createConfig(businessId));
            Task<HttpResponse> profile = repository.Get();

            return profile;
        });
    }
}