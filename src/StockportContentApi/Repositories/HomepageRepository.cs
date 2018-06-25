using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using Contentful.Core.Search;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Client;
using StockportContentApi.ContentfulFactories;
using System.Linq;

namespace StockportContentApi.Repositories
{
    public class HomepageRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulHomepage, Homepage> _homepageFactory;


        public HomepageRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulHomepage, Homepage> homepageFactory)
        {
            _client = clientManager.GetClient(config);
            _homepageFactory = homepageFactory;
        }

        public async Task<HttpResponse> Get()
        {
            var builder = new QueryBuilder<ContentfulHomepage>().ContentTypeIs("homepage").Include(2);
            var entries = await _client.GetEntries(builder);
            var entry = entries.FirstOrDefault();

            return entry == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No homepage found")
                : HttpResponse.Successful(_homepageFactory.ToModel(entry));
        }
    }
}
