namespace StockportContentApi.Controllers;

[ApiController]
public class DirectoryController : Controller
{
    private readonly Func<string, DirectoryRepository> _createDirectoryRepository;
    private readonly ResponseHandler _handler;

    public DirectoryController(ResponseHandler handler,
        Func<string, DirectoryRepository> createDirectoryRepository)
    {
        _handler = handler;
        _createDirectoryRepository = createDirectoryRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/directories")]
    public async Task<IActionResult> GetDirectories(string businessId) =>
        await _handler.Get(() =>
        {
            DirectoryRepository directoryRepository = _createDirectoryRepository(businessId);
            return directoryRepository.Get();
        });

    [HttpGet]
    [Route("v1/{businessId}/directory/{slug}")]
    public async Task<IActionResult> GetDirectory(string slug, string businessId)
    {
        try
        {
            DirectoryRepository directoryRepository = _createDirectoryRepository(businessId);
            HttpResponse response = await directoryRepository.Get(slug);

            return response.CreateResult();
        }
        catch
        {
            return new StatusCodeResult(500);
        }
    }

    [HttpGet]
    [Route("v1/{businessId}/directory-entry/{directoryEntrySlug}")]
    public async Task<IActionResult> GetDirectoryEntry(string directoryEntrySlug, string businessId)
        => await _handler.Get(() =>
        {
            DirectoryRepository directoryRepository = _createDirectoryRepository(businessId);

            return directoryRepository.GetEntry(directoryEntrySlug);
        });
}