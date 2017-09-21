using System;
using StockportContentApi.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;

namespace StockportContentApi.Controllers
{
    public class FooterController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, FooterRepository> _createRepository;

        public FooterController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, FooterRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/footer")]
        [Route("api/v1/{businessId}/footer")]
        public async Task<IActionResult> GetFooter(string businessId)
        {
            var response = await _handler.Get(() =>
            {
                var footerRepository = _createRepository(_createConfig(businessId));
                return footerRepository.GetFooter();
            });

            return response;
        }
    }}