namespace StockportContentApi.Controllers;

public class GroupAdvisorController(Func<string, IGroupAdvisorRepository> createRepository) : Controller
{
    private readonly Func<string, IGroupAdvisorRepository> _createRepository = createRepository;

    [HttpGet]
    [Route("{businessId}/groups/{slug}/advisors/")]
    [Route("v1/{businessId}/groups/{slug}/advisors/")]
    public async Task<IActionResult> GroupAdvisorsByGroup(string businessId, string slug)
    {
        IGroupAdvisorRepository repository = _createRepository(businessId);
        List<GroupAdvisor> result = await repository.GetAdvisorsByGroup(slug);

        if (result is null || !result.Any())
            return new NotFoundObjectResult($"No group advisors found for group {slug}");

        return new OkObjectResult(result);
    }

    [HttpGet]
    [Route("{businessId}/groups/advisors/{email}")]
    [Route("v1/{businessId}/groups/advisors/{email}")]
    public async Task<IActionResult> GetGroupAdvisorsByEmail(string businessId, string email)
    {
        IGroupAdvisorRepository repository = _createRepository(businessId);
        GroupAdvisor result = await repository.Get(email);

        if (result is null)
            return new NotFoundObjectResult($"No group advisor found for email {email}");

        return new OkObjectResult(result);
    }

    [HttpGet]
    [Route("{businessId}/groups/{slug}/advisors/{email}")]
    [Route("v1/{businessId}/groups/{slug}/advisors/{email}")]
    public async Task<IActionResult> CheckIfUserHasAccessToGroupBySlug(string businessId, string email, string slug)
    {
        IGroupAdvisorRepository repository = _createRepository(businessId);
        bool result = await repository.CheckIfUserHasAccessToGroupBySlug(slug, email);

        if (!result)
            return new NotFoundObjectResult($"Email {email} doesn't have access to group {slug}'s advisor console");

        return new OkObjectResult(result);
    }
}