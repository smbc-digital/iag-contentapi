using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;


namespace StockportContentApi.Controllers
{
    public class SmartAnswersController : Controller
    {
        private ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, SmartAnswersRepository> _smartAnswersRepository;

        public SmartAnswersController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, SmartAnswersRepository> smartAnswersRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _smartAnswersRepository = smartAnswersRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/smart/{slug}")]
        public async Task<IActionResult> Index(string businessId, string slug)
        {
            return await _handler.Get(() =>
            {
                var repository = _smartAnswersRepository(_createConfig(businessId));
                return repository.Get(slug);
            });
        }
    }
}
