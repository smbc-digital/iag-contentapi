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
        private readonly Func<ContentfulConfig, GroupCategoryRepository> _groupCategoryRepository;

        public GroupController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, GroupRepository> createRepository,
            Func<ContentfulConfig, GroupCategoryRepository> groupCategoryRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
            _groupCategoryRepository = groupCategoryRepository;
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

        [HttpGet]
        [Route("api/{businessId}/groupCategory")]
        public async Task<IActionResult> GetGroupCategories(string businessId)
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _groupCategoryRepository(_createConfig(businessId));
                return groupRepository.GetGroupCategories();
            });
        }

        [HttpGet]
        [Route("api/{businessId}/groupResults")]
        public async Task<IActionResult> GetGroupResults(string businessId, [FromQuery] string category = "")
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _createRepository(_createConfig(businessId));
                return groupRepository.GetGroupResults(category);
            });
        }
    }
}
