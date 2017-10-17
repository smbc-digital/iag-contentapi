using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    public class StartPageController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, StartPageRepository> _createRepository;

        public StartPageController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, StartPageRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("{businessId}/start-page/{slug}")]
        [Route("v1/{businessId}/start-page/{slug}")]
        public async Task<IActionResult> GetStartPage(string slug, string businessId)
        {
            return await _handler.Get(() =>
            {
                var startPageRepository = _createRepository(_createConfig(businessId));
                return startPageRepository.GetStartPage(slug);
            });
        }

        [HttpGet]
        [Route("{businessId}/start-page/")]
        [Route("v1/{businessId}/start-page/")]
        public async Task<IActionResult> Get(string businessId)
        {
            return await _handler.Get(() =>
            {
                var startRepository = _createRepository(_createConfig(businessId));
                return startRepository.Get();
            });
        }
    }
}
