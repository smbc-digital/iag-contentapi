using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportContentApi.Config;
using StockportContentApi.Exceptions;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CacheController : Controller
    {
        private readonly ICache _cache;
        private readonly ILogger<CacheController> _logger;

        public CacheController(ICache cache, ILogger<CacheController> logger)
        {
            _cache = cache;
            _logger = logger;
        }

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
            catch(CacheException)
            {
                _logger.LogError("Error deleting key from cache");
                return StatusCode(500);
            }

            return Ok();
        }
    }
}
