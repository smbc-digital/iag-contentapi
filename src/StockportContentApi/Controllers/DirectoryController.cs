namespace StockportContentApi.Controllers;

[ApiController]
public class DirectoryController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, DirectoryRepository> _createDirectoryRepository;
    private readonly ResponseHandler _handler;

    public DirectoryController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, DirectoryRepository> createDirectoryRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createDirectoryRepository = createDirectoryRepository;
    }

    [HttpGet]
    [Route("{businessId}/directories")]
    [Route("v2/{businessId}/directories")]
    public async Task<IActionResult> GetDirectories(string businessId)
    {
        return await _handler.Get(() =>
        {
            DirectoryRepository directoryRepository = _createDirectoryRepository(_createConfig(businessId));
            
            return directoryRepository.Get();
        });
    }

    [HttpGet]
    [Route("{businessId}/directory/{slug}")]
    [Route("v2/{businessId}/directory/{slug}")]
    public async Task<IActionResult> GetDirectory(string slug, string businessId)
    {
        try
        {
            DirectoryRepository directoryRepository = _createDirectoryRepository(_createConfig(businessId));
            HttpResponse response = await directoryRepository.Get(slug);

            return response.CreateResult();
        }
        catch
        {
            return new StatusCodeResult(500);
        }
    }

    [HttpGet]
    [Route("{businessId}/directory-entry/{directoryEntrySlug}")]
    [Route("v2/{businessId}/directory-entry/{directoryEntrySlug}")]
    public async Task<IActionResult> GetDirectoryEntry(string directoryEntrySlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            DirectoryRepository directoryRepository = _createDirectoryRepository(_createConfig(businessId));

            return directoryRepository.GetEntry(directoryEntrySlug);
        });
    }
}