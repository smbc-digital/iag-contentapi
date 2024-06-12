namespace StockportContentApi.Controllers;

public class ProfileController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly Func<ContentfulConfig, ProfileRepository> _createRepository;
    private readonly Func<string, ContentfulConfig> _createConfig;

    public ProfileController(ResponseHandler handler, Func<string, ContentfulConfig> createConfig, Func<ContentfulConfig, ProfileRepository> createRepository)
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
            var repository = _createRepository(_createConfig(businessId));
            var profile = repository.GetProfile(profileSlug);

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
            var repository = _createRepository(_createConfig(businessId));
            var profile = repository.Get();

            return profile;
        });
    }
}