using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    public class AtoZController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, AtoZRepository> _createRepository;
        public AtoZController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, AtoZRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("/api/{businessId}/atoz/{letter}")]
        public async Task<IActionResult> Index(string letter, string businessId)
        {
            return await _handler.Get(() =>
            {
                var repository = _createRepository(_createConfig(businessId));
                return repository.Get(letter);
            });
        }
    }
}
