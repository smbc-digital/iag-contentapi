namespace StockportContentApi.Repositories;

public interface IGroupAdvisorRepository
{
    Task<List<GroupAdvisor>> GetAdvisorsByGroup(string slug);
    Task<GroupAdvisor> Get(string email);
    Task<bool> CheckIfUserHasAccessToGroupBySlug(string slug, string email);
}

public class GroupAdvisorRepository(ContentfulConfig config,
                                    IContentfulClientManager contentfulClientManager,
                                    IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor> contentfulFactory) : IGroupAdvisorRepository
{
    readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    readonly IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor> _contentfulFactory = contentfulFactory;

    public async Task<List<GroupAdvisor>> GetAdvisorsByGroup(string slug)
    {
        QueryBuilder<ContentfulGroupAdvisor> builder = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").Include(1);
        ContentfulCollection<ContentfulGroupAdvisor> entries = await _client.GetEntries(builder);

        if (entries is null)
            return new List<GroupAdvisor>();

        List<GroupAdvisor> result = entries.Select(_contentfulFactory.ToModel).ToList();

        return result.Where(item => item.Groups.Contains(slug)).ToList();
    }

    public async Task<GroupAdvisor> Get(string email)
    {
        QueryBuilder<ContentfulGroupAdvisor> builder = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", email).Include(1);
        ContentfulCollection<ContentfulGroupAdvisor> entries = await _client.GetEntries(builder);

        if (entries is null)
            return null;

        return entries.Select(_contentfulFactory.ToModel).FirstOrDefault();
    }

    public async Task<bool> CheckIfUserHasAccessToGroupBySlug(string slug, string email)
    {
        QueryBuilder<ContentfulGroupAdvisor> builder = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", email).Include(1);
        ContentfulCollection<ContentfulGroupAdvisor> entries = await _client.GetEntries(builder);

        if (entries is null)
            return false;

        GroupAdvisor result = _contentfulFactory.ToModel(entries.FirstOrDefault());

        if (result.HasGlobalAccess || result.Groups.Contains(slug))
            return true;

        return false;
    }
}