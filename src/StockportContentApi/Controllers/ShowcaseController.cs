using StockportContentApi.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;

namespace StockportContentApi.Controllers
{
    public class ShowcaseController : Controller
    {
        
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, ShowcaseRepository> _createRepository;

        public ShowcaseController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, ShowcaseRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/showcases/{showcaseSlug}")]
        [Route("api/v1/{businessId}/showcases/{showcaseSlug}")]
        public async Task<IActionResult> GetShowcase(string showcaseSlug, string  businessId)
        {
            return await _handler.Get(() =>
            {
                var repository = _createRepository(_createConfig(businessId));
                var showcase = repository.GetShowcases(showcaseSlug);

                return showcase;
            });
        }

        [HttpGet]
        [Route("api/{businessId}/showcases/")]
        [Route("api/v1/{businessId}/showcases/")]
        public async Task<IActionResult> Get(string businessId)
        {
            return await _handler.Get(() =>
            {
                var repository = _createRepository(_createConfig(businessId));
                var showcase = repository.Get();

                return showcase;
            });
        }
    }
}