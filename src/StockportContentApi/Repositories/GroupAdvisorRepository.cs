using Contentful.Core;
using StockportContentApi.Client;
using StockportContentApi.Config;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.ContentfulModels;
using System.Linq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Model;
using System.Collections.Generic;

namespace StockportContentApi.Repositories
{
    public interface IGroupAdvisorRepository
    {
        Task<List<GroupAdvisor>> GetAdvisorsByGroup(string slug);
        Task<GroupAdvisor> Get(string email);
        Task<bool> CheckIfUserHasAccessToGroupBySlug(string slug, string email);
    }

    public class GroupAdvisorRepository : IGroupAdvisorRepository
    {
        readonly IContentfulClient _client;
        readonly IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor> _contentfulFactory;

        public GroupAdvisorRepository(ContentfulConfig config, IContentfulClientManager contentfulClientManager, IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor> contentfulFactory)
        {
            _client = contentfulClientManager.GetClient(config);
            _contentfulFactory = contentfulFactory;
        }

        public async Task<List<GroupAdvisor>> GetAdvisorsByGroup(string slug)
        {
            var builder = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").Include(1);

            var entries = await _client.GetEntries(builder);

            if (entries == null) return new List<GroupAdvisor>();

            var result = entries.Select(item => _contentfulFactory.ToModel(item)).ToList();

            return result.Where(item => item.Groups.Contains(slug)).ToList();
        }

        public async Task<GroupAdvisor> Get(string email)
        {
            var builder = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", email).Include(1);

            var entries = await _client.GetEntries(builder);

            if (entries == null) return null;

            return entries.Select(item => _contentfulFactory.ToModel(item)).FirstOrDefault();
        }

        public async Task<bool> CheckIfUserHasAccessToGroupBySlug(string slug, string email)
        {
            var builder = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", email).Include(1);

            var entries = await _client.GetEntries(builder);

            if (entries == null) return false;

            var result = _contentfulFactory.ToModel(entries.FirstOrDefault());

            if (result.HasGlobalAccess || result.Groups.Contains(slug)) return true;

            return false;
        }
    }
}
