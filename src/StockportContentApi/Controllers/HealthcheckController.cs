using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Services;
using System.Threading.Tasks;

namespace StockportContentApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HealthcheckController : Controller
    {
        private readonly IHealthcheckService _healthcheckService;

        public HealthcheckController(IHealthcheckService healthcheckService)
        {
            _healthcheckService = healthcheckService;
        }

        [HttpGet]
        [Route("/_healthcheck")]
        public async Task<IActionResult> Index(string articleSlug, string businessId)
        {
            return await Task.Run(async () =>Json(await _healthcheckService.Get()));
        }
    }
}
