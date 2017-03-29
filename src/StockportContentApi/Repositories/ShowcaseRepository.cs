using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class ShowcaseRepository
    {
        private readonly IContentfulFactory<ContentfulShowcase, Showcase> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public ShowcaseRepository(ContentfulConfig config, IContentfulFactory<ContentfulShowcase, Showcase> showcaseBuilder, IContentfulClientManager contentfulClientManager)
        {
            _contentfulFactory = showcaseBuilder;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetShowcases(string slug)
        {
            var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);

            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();
            var showcase = _contentfulFactory.ToModel(entry);

            return showcase.GetType() == typeof(NullHomepage) 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Showcase found") 
                : HttpResponse.Successful(showcase);
        }
    }
}
