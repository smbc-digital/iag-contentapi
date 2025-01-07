namespace StockportContentApi.Controllers;

public class ProfileController(ResponseHandler handler,
                            Func<string, IProfileRepository> createRepository) : Controller
{
    private readonly Func<string, IProfileRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/profiles/{profileSlug}")]
    [Route("v1/{businessId}/profiles/{profileSlug}")]
    public async Task<IActionResult> GetProfile(string profileSlug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetProfile(profileSlug));

    [HttpGet]
    [Route("{businessId}/profiles/")]
    [Route("v1/{businessId}/profiles/")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() => _createRepository(businessId).Get());
}