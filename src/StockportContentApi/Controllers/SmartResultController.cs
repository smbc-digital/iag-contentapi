using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Services;

namespace StockportContentApi.Controllers
{
    public class SmartResultController : Controller
    {
        private readonly ISmartResultService _smartResultService;

        public SmartResultController(ISmartResultService smartResultService)
        {
            _smartResultService = smartResultService;
        }

        [Route("{businessId}/smart-result/{slug}")]
        public async Task<IActionResult> GetSmartResult(string businessId, string slug)
        {
            var response = await _smartResultService.GetSmartResultBySlug(businessId, slug);
            return new OkObjectResult(response);

        }
    }
}
