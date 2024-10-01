namespace StockportContentApi.Controllers;

public class GroupController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, IGroupRepository> _groupRepository;
    private readonly Func<ContentfulConfig, EventRepository> _eventRepository;
    private readonly Func<ContentfulConfig, GroupCategoryRepository> _groupCategoryRepository;
    private readonly Func<ContentfulConfig, ManagementRepository> _managementRepository;
    private readonly IMapper _mapper;

    public GroupController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, IGroupRepository> groupRepository,
        Func<ContentfulConfig, EventRepository> eventRepository,
        Func<ContentfulConfig, GroupCategoryRepository> groupCategoryRepository,
        Func<ContentfulConfig, ManagementRepository> managementRepository,
        IMapper mapper
        )
    {
        _handler = handler;
        _createConfig = createConfig;
        _groupRepository = groupRepository;
        _eventRepository = eventRepository;
        _groupCategoryRepository = groupCategoryRepository;
        _managementRepository = managementRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("{businessId}/groups")]
    [Route("v1/{businessId}/groups")]
    public async Task<IActionResult> GetGroups(string businessId)
    {
        return await _handler.Get(() =>
        {
            IGroupRepository groupRepository = _groupRepository(_createConfig(businessId));
            return groupRepository.Get();
        });
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("{businessId}/grouphomepage")]
    [Route("v1/{businessId}/grouphomepage")]
    public async Task<IActionResult> Homepage(string businessId)
    {
        IGroupRepository repository = _groupRepository(_createConfig(businessId));
        HttpResponse response = await repository.GetGroupHomepage();
        GroupHomepage homepage = response.Get<GroupHomepage>();
        return Ok(homepage);
    }

    [HttpGet]
    [Route("{businessId}/groups/{groupSlug}")]
    [Route("v1/{businessId}/groups/{groupSlug}")]
    public async Task<IActionResult> GetGroup(string groupSlug, string businessId, [FromQuery] bool onlyActive = true)
    {
        return await _handler.Get(() =>
        {
            IGroupRepository groupRepository = _groupRepository(_createConfig(businessId));
            return groupRepository.GetGroup(groupSlug, onlyActive);
        });
    }

    [HttpGet]
    [Route("{businessId}/group-categories")]
    [Route("v1/{businessId}/group-categories")]
    public async Task<IActionResult> GetGroupCategories(string businessId)
    {
        return await _handler.Get(() =>
        {
            GroupCategoryRepository groupRepository = _groupCategoryRepository(_createConfig(businessId));
            return groupRepository.GetGroupCategories();
        });
    }

    [HttpGet]
    [Route("{businessId}/group-results")]
    [Route("v1/{businessId}/group-results")]
    public async Task<IActionResult> GetGroupResults(string businessId, GroupSearch groupSearch, [FromQuery] string slugs = "")
    {
        return await _handler.Get(() =>
        {
            IGroupRepository groupRepository = _groupRepository(_createConfig(businessId));
            return groupRepository.GetGroupResults(groupSearch, slugs);
        });
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("{businessId}/groups/administrators/{email}")]
    [Route("v1/{businessId}/groups/administrators/{email}")]
    public async Task<IActionResult> GetAdministratorsGroups(string businessId, string email)
    {
        return await _handler.Get(() =>
        {
            IGroupRepository groupRepository = _groupRepository(_createConfig(businessId));
            return groupRepository.GetAdministratorsGroups(email);
        });
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPut]
    [Route("{businessId}/groups/{slug}")]
    [Route("v1/{businessId}/groups/{slug}")]
    public async Task<IActionResult> UpdateGroup([FromBody] Group group, string businessId)
    {
        try
        {
            IGroupRepository groupRepository = _groupRepository(_createConfig(businessId));
            ContentfulGroup existingGroup = await groupRepository.GetContentfulGroup(group.Slug);

            ContentfulCollection<ContentfulGroupCategory> existingCategories = await groupRepository.GetContentfulGroupCategories();
            List<ContentfulGroupCategory> referencedCategories = existingCategories.Where(c => group.CategoriesReference.Select(cr => cr.Slug).Contains(c.Slug)).ToList();

            ManagementGroup managementGroup = ConvertToManagementGroup(group, referencedCategories, existingGroup);

            return await _handler.Get(async () =>
            {
                ManagementRepository managementRepository = _managementRepository(_createConfig(businessId));
                int version = await managementRepository.GetVersion(existingGroup.Sys.Id);
                existingGroup.Sys.Version = version;
                return await managementRepository.CreateOrUpdate(managementGroup, existingGroup.Sys);
            });
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpDelete]
    [Route("{businessId}/groups/{slug}")]
    [Route("v1/{businessId}/groups/{slug}")]
    public async Task<IActionResult> DeleteGroup(string slug, string businessId)
    {
        IGroupRepository repository = _groupRepository(_createConfig(businessId));
        EventRepository eventRepository = _eventRepository(_createConfig(businessId));
        ContentfulGroup existingGroup = await repository.GetContentfulGroup(slug);
        IEnumerable<ContentfulEvent> groupEvents = await eventRepository.GetAllEventsForAGroup(slug);

        return await _handler.Get(async () =>
        {
            ManagementRepository managementRepository = _managementRepository(_createConfig(businessId));

            foreach (ContentfulEvent groupEvent in groupEvents)
            {
                int eventVersion = await managementRepository.GetVersion(groupEvent.Sys.Id);
                groupEvent.Sys.Version = eventVersion;
                HttpResponse result = await managementRepository.Delete(groupEvent.Sys);
                if (result.StatusCode is not HttpStatusCode.OK)
                    return result;
            }

            int version = await managementRepository.GetVersion(existingGroup.Sys.Id);
            existingGroup.Sys.Version = version;
            return await managementRepository.Delete(existingGroup.Sys);
        });
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpDelete]
    [Route("{businessId}/groups/{slug}/administrators/{emailAddress}")]
    [Route("v1/{businessId}/groups/{slug}/administrators/{emailAddress}")]
    public async Task<IActionResult> RemoveAdministrator(string slug, string emailAddress, string businessId)
    {
        IGroupRepository repository = _groupRepository(_createConfig(businessId));

        ContentfulGroup existingGroup = await repository.GetContentfulGroup(slug);

        existingGroup.GroupAdministrators.Items = existingGroup.GroupAdministrators.Items.Where(a => !a.Email.Equals(emailAddress)).ToList();

        ManagementGroup managementGroup = new();
        _mapper.Map(existingGroup, managementGroup);

        return await _handler.Get(async () =>
        {
            ManagementRepository managementRepository = _managementRepository(_createConfig(businessId));
            int version = await managementRepository.GetVersion(existingGroup.Sys.Id);
            existingGroup.Sys.Version = version;
            HttpResponse response = await managementRepository.CreateOrUpdate(managementGroup, existingGroup.Sys);

            return response.StatusCode is HttpStatusCode.OK 
                ? HttpResponse.Successful("Successfully deleted administrator") 
                : HttpResponse.Failure(response.StatusCode, "An error has occurred");
        });
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPut]
    [Route("{businessId}/groups/{slug}/administrators/{emailAddress}")]
    [Route("v1/{businessId}/groups/{slug}/administrators/{emailAddress}")]
    public async Task<IActionResult> UpdateAdministrator([FromBody] GroupAdministratorItems user, string slug, string emailAddress, string businessId)
        => await AddOrUpdateAdministrator(user, slug, emailAddress, businessId);

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost]
    [Route("{businessId}/groups/{slug}/administrators/{emailAddress}")]
    [Route("v1/{businessId}/groups/{slug}/administrators/{emailAddress}")]
    public async Task<IActionResult> AddAdministrator([FromBody] GroupAdministratorItems user, string slug, string emailAddress, string businessId)
        => await AddOrUpdateAdministrator(user, slug, emailAddress, businessId);


    private async Task<IActionResult> AddOrUpdateAdministrator(GroupAdministratorItems user, string slug, string emailAddress, string businessId)
    {
        IGroupRepository repository = _groupRepository(_createConfig(businessId));

        ContentfulGroup existingGroup = await repository.GetContentfulGroup(slug);

        existingGroup.GroupAdministrators.Items = existingGroup.GroupAdministrators.Items.Where(a => !a.Email.Equals(emailAddress)).ToList();

        if (user is not null)
        {
            existingGroup.GroupAdministrators.Items.Add(user);

            ManagementGroup managementGroup = new();
            _mapper.Map(existingGroup, managementGroup);

            return await _handler.Get(async () =>
            {
                ManagementRepository managementRepository = _managementRepository(_createConfig(businessId));
                int version = await managementRepository.GetVersion(existingGroup.Sys.Id);
                existingGroup.Sys.Version = version;
                HttpResponse response = await managementRepository.CreateOrUpdate(managementGroup, existingGroup.Sys);
                return response.StatusCode is HttpStatusCode.OK 
                    ? HttpResponse.Successful($"Successfully updated the group {existingGroup.Name}") 
                    : HttpResponse.Failure(response.StatusCode, "An error has occurred");
            });
        }
        else
            throw new Exception("Invalid data received");
    }

    private ManagementGroup ConvertToManagementGroup(Group group, List<ContentfulGroupCategory> referencedCategories, ContentfulGroup existingGroup)
    {
        ContentfulGroup contentfulGroup = _mapper.Map<ContentfulGroup>(group);
        contentfulGroup.GroupBranding = existingGroup.GroupBranding;
        contentfulGroup.CategoriesReference = referencedCategories;
        contentfulGroup.Image = existingGroup.Image;
        contentfulGroup.SubCategories = existingGroup.SubCategories;
        contentfulGroup.Organisation = existingGroup.Organisation;
        ManagementGroup managementGroup = new();
        _mapper.Map(contentfulGroup, managementGroup);

        return managementGroup;
    }
}
