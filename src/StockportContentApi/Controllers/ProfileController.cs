using StockportContentApi.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;

namespace StockportContentApi.Controllers
{
    public class ProfileController : Controller
    {
        
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, ProfileRepository> _createRepository;

        public ProfileController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, ProfileRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/profiles/{profileSlug}")]
        [Route("api/v1/{businessId}/profiles/{profileSlug}")]
        public async Task<IActionResult> GetProfile(string profileSlug, string  businessId)
        {
            return await _handler.Get(() =>
            {
                var profileRepository = _createRepository(_createConfig(businessId));
                return profileRepository.GetProfile(profileSlug);
            });
        }

        [HttpGet]
        [Route("api/{businessId}/profiles/")]
        [Route("api/v1/{businessId}/profiles/")]
        public async Task<IActionResult> Get(string businessId)
        {
            return await _handler.Get(() =>
            {
                var profilerRepository = _createRepository(_createConfig(businessId));
                return profilerRepository.Get();
            });
        }
    }
}