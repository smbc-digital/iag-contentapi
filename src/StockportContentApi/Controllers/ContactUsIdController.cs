using StockportContentApi.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using StockportContentApi.Config;

namespace StockportContentApi.Controllers
{
    public class ContactUsIdController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, ContactUsIdRepository> _createRepository;

        public ContactUsIdController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, ContactUsIdRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("/api/{businessId}/ContactUsId/{slug}")]
        public async Task<IActionResult> Detail(string slug, string businessId)
        {
            return await _handler.Get(() =>
            {
                var contactUsIdRepository = _createRepository(_createConfig(businessId));
                return contactUsIdRepository.GetContactUsIds(slug);
            });
        }
    }
}