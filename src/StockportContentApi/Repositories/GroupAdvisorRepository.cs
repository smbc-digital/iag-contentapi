using Contentful.Core;
using StockportContentApi.Client;
using StockportContentApi.Config;
using System.Threading.Tasks;
using StockportContentApi.Http;

namespace StockportContentApi.Repositories
{
    public class GroupAdvisorRepository
    {
        readonly IContentfulClient _client;

        public GroupAdvisorRepository(ContentfulConfig config, IContentfulClientManager contentfulClientManager)
        {
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetAdvisorsByGroup(string slug)
        {
            return null;
        }

        public async Task<HttpResponse> Get(string email)
        {
            return null;
        }

        public async Task<HttpResponse> CheckIfUserHasAccessToGroupBySlug(string slug, string email)
        {
            return null;
        }
    }
}
