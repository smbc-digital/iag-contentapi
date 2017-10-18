using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;
using System;
using System.Threading.Tasks;

namespace StockportContentApi.Controllers
{
    public class GroupAdvisorController : Controller
    {
        readonly ResponseHandler _handler;
        readonly Func<string, ContentfulConfig> _createConfig;
        readonly Func<ContentfulConfig, GroupAdvisorRepository> _createRepository;

        public GroupAdvisorController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, GroupAdvisorRepository> createRepository)
        {
            _createRepository = createRepository;
            _handler = handler;
            _createConfig = createConfig;
        }

        [HttpGet]
        [Route("{businessId}/groups/{slug}/advisors/")]
        [Route("v1/{businessId}/groups/{slug}/advisors/")]
        public async Task<IActionResult> GroupAdvisorsByGroup(string businessId, string slug)
        {
            return await _handler.Get(() =>
            {
                var repository = _createRepository(_createConfig(businessId));
                return repository.GetAdvisorsByGroup(slug);
            });
        }

        [HttpGet]
        [Route("{businessId}/groups/advisors/{email}")]
        [Route("v1/{businessId}/groups/advisors/{email}")]
        public async Task<IActionResult> GetGroupAdvisorsByEmail(string businessId, string email)
        {
            return await _handler.Get(() =>
            {
                var repository = _createRepository(_createConfig(businessId));
                return repository.Get(email);
            });
        }

        [HttpGet]
        [Route("{businessId}/groups/{slug}/advisors/{email}")]
        [Route("v1/{businessId}/groups/{slug}/advisors/{email}")]
        public async Task<IActionResult> CheckIfUserHasAccessToGroupBySlug(string businessId, string email, string slug)
        {
            return await _handler.Get(() =>
            {
                var repository = _createRepository(_createConfig(businessId));
                return repository.CheckIfUserHasAccessToGroupBySlug(slug, email);
            });
        }
    }
}
