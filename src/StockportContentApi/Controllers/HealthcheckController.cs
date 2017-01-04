using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Services;
using System.Threading.Tasks;

namespace StockportContentApi.Controllers
{
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
            return await Task.Run( () =>Json(_healthcheckService.Get()));
        }
    }
}
