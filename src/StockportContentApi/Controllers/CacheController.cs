namespace StockportContentApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class CacheController(ICache cache, ILogger<CacheController> logger) : Controller
{
    private readonly ICache _cache = cache;
    private readonly ILogger<CacheController> _logger = logger;

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("{businessId}/clearcache/{cacheKey}")]
    [Route("v1/{businessId}/clearcache/{cacheKey}")]
    public IActionResult ClearCache(string cacheKey)
    {
        try
        {
            _cache.RemoveItemFromCache(cacheKey);
        }
        catch (CacheException)
        {
            _logger.LogError("Error deleting key from cache");
            return StatusCode(500);
        }

        return Ok();
    }
}