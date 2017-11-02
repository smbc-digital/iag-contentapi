using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportContentApi.Exceptions;
using StockportContentApi.Services;

namespace StockportContentApi.Controllers
{
    public class SmartResultController : Controller
    {
        private readonly ISmartResultService _smartResultService;
        private readonly ILogger<SmartResultController> _logger;

        public SmartResultController(ISmartResultService smartResultService, ILogger<SmartResultController> logger)
        {
            _smartResultService = smartResultService;
            _logger = logger;
        }

        [Route("{businessId}/smart-result/{slug}")]
        public async Task<IActionResult> GetSmartResult(string businessId, string slug)
        {
            try
            {
                var response = await _smartResultService.GetSmartResultBySlug(businessId, slug);
                return new OkObjectResult(response);
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Error getting smart-result with slug: {slug} with exception: {ex.Message}");
                return StatusCode(500);
            }

        }
    }
}
