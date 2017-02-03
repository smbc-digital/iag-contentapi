using StockportContentApi.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;

namespace StockportContentApi.Controllers
{
    public class GroupController
    {

        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, GroupRepository> _createRepository;

        public GroupController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, GroupRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/group/{groupSlug}")]
        public async Task<IActionResult> GetGroup(string groupSlug, string businessId)
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _createRepository(_createConfig(businessId));
                return groupRepository.GetGroup(groupSlug);
            });
        }
    }
}
