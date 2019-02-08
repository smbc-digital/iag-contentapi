using StockportContentApi.Model;
using StockportContentApi.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace StockportContentApi.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, IProfileRepository> _createRepository;

        public ProfileService(Func<string, ContentfulConfig> createConfig, Func<ContentfulConfig, IProfileRepository> createRepository)
        {
            _createRepository = createRepository;
            _createConfig = createConfig;
        }

        public async Task<Model.Profile> GetProfile(string slug, string businessId)
        {
            var profileRepository = _createRepository(_createConfig(businessId));
            var response = await profileRepository.GetProfile(slug);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Get<Model.Profile>();
            }

            return null;
        }

        public async Task<List<Model.Profile>> GetProfiles(string businessId)
        {
            var profileRepository = _createRepository(_createConfig(businessId));
            var response = await profileRepository.Get();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var profiles = response.Get<IEnumerable<Model.Profile>>();
                return profiles.ToList();
            }

            return null;
        }
    }
}