using StockportContentApi.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;

namespace StockportContentApi.Controllers
{
    public class ProfileController
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
        [Route("api/{businessId}/profile/{profileSlug}")]
        public async Task<IActionResult> GetProfile(string profileSlug, string  businessId)
        {
            return await _handler.Get(() =>
            {
                var profileRepository = _createRepository(_createConfig(businessId));
                return profileRepository.GetProfile(profileSlug);
            });
        }
    }
}