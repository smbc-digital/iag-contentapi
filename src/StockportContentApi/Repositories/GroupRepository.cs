using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using System.Linq;
using StockportContentApi.Client;
using StockportContentApi.Factories;

namespace StockportContentApi.Repositories
{
    public class GroupRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulGroup, Group> _groupFactory;

        public GroupRepository(ContentfulConfig config, IContentfulClientManager clientManager, 
                                 IContentfulFactory<ContentfulGroup, Group> groupFactory)
        {
            _client = clientManager.GetClient(config);
            _groupFactory = groupFactory;
        }

        public async Task<HttpResponse> GetGroup(string slug)
        {
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(2);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            return entry == null 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No group found for '{slug}'") 
                : HttpResponse.Successful(_groupFactory.ToModel(entry));
        }
    }
}
