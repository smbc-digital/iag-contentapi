using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class GroupAdvisorContentfulFactory : IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupAdvisorContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public GroupAdvisor ToModel(ContentfulGroupAdvisor entry)
        {
            var name = entry.Name;
            var emailAddress = entry.EmailAddress;
            var groups = entry.Groups.Select(g => g.Name);
            var hasGlobalAccess = entry.HasGlobalAccess;
            


            return new GroupAdvisor(name, emailAddress, groups, hasGlobalAccess);
        }
    }
}