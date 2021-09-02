using StockportContentApi.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockportContentApi.ContentfulModels;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Model;

namespace StockportContentApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RedirectsController : Controller
    {     
        private readonly ResponseHandler _handler;
        private readonly RedirectsRepository _repository;
        private readonly ILogger<RedirectsController> _logger;
        private readonly IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects> _contenfulFactory;

        public RedirectsController(ResponseHandler handler,
            RedirectsRepository repository,
            ILogger<RedirectsController> logger,
            IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects> contentfulFactory)
        {
            _handler = handler;
            _repository = repository;
            _logger = logger;
            _contenfulFactory = contentfulFactory;
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
        public async Task<IActionResult> UpdateRedirects()
        {
            var response = await _repository.GetUpdatedRedirects();
            

            //_logger.LogWarning($"RedirectsController:: UpdateRedirects body received: {JsonConvert.SerializeObject(body)}");
            _logger.LogWarning($"RedirectsController:: UpdateRedirects converted model: {JsonConvert.SerializeObject(response)}");

            return Ok();
        }
    }
}