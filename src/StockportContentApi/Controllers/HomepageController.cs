using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    public class HomepageController : Controller
    {

        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, HomepageRepository> _createRepository;
        public HomepageController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, HomepageRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/homepage")]
        [Route("api/v1/{businessId}/homepage")]
        public async Task<IActionResult> Get(string businessId)
        {
            return await _handler.Get(() =>
            {
                var repository = _createRepository(_createConfig(businessId));
                return repository.Get();
            });
        }
    }
}
