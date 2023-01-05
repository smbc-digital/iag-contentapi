using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
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
        [Route("{businessId}/contact-us-id/{slug}")]
        [Route("v1/{businessId}/contact-us-id/{slug}")]
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