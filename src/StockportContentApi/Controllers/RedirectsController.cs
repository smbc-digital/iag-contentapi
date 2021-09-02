using StockportContentApi.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportContentApi.ContentfulModels;

namespace StockportContentApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RedirectsController : Controller
    {     
        private readonly ResponseHandler _handler;
        private readonly RedirectsRepository _repository;
        private readonly ILogger<RedirectsController> _logger;

        public RedirectsController(ResponseHandler handler,
            RedirectsRepository repository,
            ILogger<RedirectsController> logger)
        {
            _handler = handler;
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [Route("redirects")]
        [Route("v1/redirects")]
        public async Task<IActionResult> GetRedirects(string businessId)
        {
            return await _handler.Get(() => _repository.GetRedirects());
        }

        [HttpPost]
        [Route("update-redirects")]
        public async Task<IActionResult> UpdateRedirects(ContentfulRedirect body)
        {
            _logger.LogWarning($"RedirectsController:: UpdateRedirects body received: {body}");
            return Ok();
        }
    }
}