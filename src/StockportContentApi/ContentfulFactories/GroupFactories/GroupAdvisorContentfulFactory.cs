namespace StockportContentApi.ContentfulFactories.GroupFactories;

public class GroupAdvisorContentfulFactory : IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>
{
    public GroupAdvisor ToModel(ContentfulGroupAdvisor entry)
    {
        string name = entry.Name;
        string emailAddress = entry.Email;
        IEnumerable<string> groups = entry.Groups.Select(group => group.Slug);
        bool hasGlobalAccess = entry.GlobalAccess;

        return new GroupAdvisor(name, emailAddress, groups, hasGlobalAccess);
    }
}