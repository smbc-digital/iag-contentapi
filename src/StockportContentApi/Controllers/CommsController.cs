using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CommsController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, CommsRepository> _commsRepository;

        public CommsController(ResponseHandler handler, Func<string, ContentfulConfig> createConfig, Func<ContentfulConfig, CommsRepository> commsRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _commsRepository = commsRepository;
        }

        [HttpGet]
        [Route("{businessId}/comms")]
        [Route("v1/{businessId}/comms")]
        public async Task<IActionResult> Get(string businessId) =>
            await _handler.Get(() => _commsRepository(_createConfig(businessId)).Get());
    }
}
