namespace StockportContentApi.Controllers;

[ApiController]
public class DirectoryController(ResponseHandler handler,
                                Func<string, IDirectoryRepository> createDirectoryRepository) : Controller
{
    private readonly Func<string, IDirectoryRepository> _createDirectoryRepository = createDirectoryRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/directories")]
    [Route("v1/{businessId}/directories")]
    public async Task<IActionResult> GetDirectories(string businessId) =>
        await _handler.Get(() => _createDirectoryRepository(businessId).Get(businessId));

    [HttpGet]
    [Route("{businessId}/directory/{slug}")]
    [Route("v1/{businessId}/directory/{slug}")]
    public async Task<IActionResult> GetDirectory(string slug, string businessId)
    {
        try
        {
            HttpResponse response = await _createDirectoryRepository(businessId).Get(slug, businessId);

            return response.CreateResult();
        }
        catch
        {
            return new StatusCodeResult(500);
        }
    }

    [HttpGet]
    [Route("{businessId}/directory-entry/{directoryEntrySlug}")]
    [Route("v1/{businessId}/directory-entry/{directoryEntrySlug}")]
    public async Task<IActionResult> GetDirectoryEntry(string directoryEntrySlug, string businessId) =>
        await _handler.Get(() => _createDirectoryRepository(businessId).GetEntry(directoryEntrySlug, businessId));
}