namespace StockportContentApi.Controllers;

public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly FeatureToggles _featureToggles;

    public ProfileController(IProfileService profileService, FeatureToggles featureToggles)
    {
        _profileService = profileService;
        _featureToggles = featureToggles;
    }

    [HttpGet]
    [Route("{businessId}/profiles/{profileSlug}")]
    [Route("v1/{businessId}/profiles/{profileSlug}")]
    public async Task<IActionResult> GetProfile(string profileSlug, string businessId)
    {
        var profile = await _profileService.GetProfile(profileSlug, businessId);
        return new OkObjectResult(profile);
    }

    [HttpGet]
    [Route("{businessId}/profiles/")]
    [Route("v1/{businessId}/profiles/")]
    public async Task<IActionResult> Get(string businessId)
    {
        var profiles = await _profileService.GetProfiles(businessId);

        return new OkObjectResult(profiles);
    }
}