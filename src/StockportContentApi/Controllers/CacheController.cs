namespace StockportContentApi.Controllers;

public class CacheController : Controller
{
    private readonly ICache _cache;
    private readonly ILogger<CacheController> _logger;

    public CacheController(ICache cache, ILogger<CacheController> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
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