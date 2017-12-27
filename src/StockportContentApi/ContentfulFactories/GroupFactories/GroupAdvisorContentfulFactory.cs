using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories.GroupFactories
{
    public class GroupAdvisorContentfulFactory : IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>
    {
        public GroupAdvisor ToModel(ContentfulGroupAdvisor entry)
        {
            var name = entry.Name;
            var emailAddress = entry.Email;
            var groups = entry.Groups.Select(g => g.Slug);
            var hasGlobalAccess = entry.GlobalAccess;

            return new GroupAdvisor(name, emailAddress, groups, hasGlobalAccess);
        }
    }
}