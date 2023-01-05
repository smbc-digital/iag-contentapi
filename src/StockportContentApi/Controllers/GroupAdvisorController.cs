using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    public class GroupAdvisorController : Controller
    {
        readonly Func<string, ContentfulConfig> _createConfig;
        readonly Func<ContentfulConfig, IGroupAdvisorRepository> _createRepository;

        public GroupAdvisorController(Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, IGroupAdvisorRepository> createRepository)
        {
            _createRepository = createRepository;
            _createConfig = createConfig;
        }

        [HttpGet]
        [Route("{businessId}/groups/{slug}/advisors/")]
        [Route("v1/{businessId}/groups/{slug}/advisors/")]
        public async Task<IActionResult> GroupAdvisorsByGroup(string businessId, string slug)
        {
            var repository = _createRepository(_createConfig(businessId));
            var result = await repository.GetAdvisorsByGroup(slug);

            if (result == null || !result.Any()) return new NotFoundObjectResult($"No group advisors found for group {slug}");

            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("{businessId}/groups/advisors/{email}")]
        [Route("v1/{businessId}/groups/advisors/{email}")]
        public async Task<IActionResult> GetGroupAdvisorsByEmail(string businessId, string email)
        {
            var repository = _createRepository(_createConfig(businessId));
            var result = await repository.Get(email);

            if (result == null) return new NotFoundObjectResult($"No group advisor found for email {email}");

            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("{businessId}/groups/{slug}/advisors/{email}")]
        [Route("v1/{businessId}/groups/{slug}/advisors/{email}")]
        public async Task<IActionResult> CheckIfUserHasAccessToGroupBySlug(string businessId, string email, string slug)
        {
            var repository = _createRepository(_createConfig(businessId));
            var result = await repository.CheckIfUserHasAccessToGroupBySlug(slug, email);

            if (!result) return new NotFoundObjectResult($"Email {email} doesn't have access to group {slug}'s advisor console");

            return new OkObjectResult(result);
        }
    }
}
