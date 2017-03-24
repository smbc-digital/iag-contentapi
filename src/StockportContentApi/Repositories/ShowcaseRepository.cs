using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class ShowcaseRepository
    {
        private readonly string _contentfulApiUrl;
        private readonly IContentfulFactory<ContentfulShowcase, Showcase> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public ShowcaseRepository(ContentfulConfig config, IContentfulFactory<ContentfulShowcase, Showcase> showcaseBuilder, IContentfulClientManager contentfulClientManager)
        {
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _contentfulFactory = showcaseBuilder;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetShowcases(string ShowcaseSlug)
        {
            var builder = new QueryBuilder<ContentfulShowcase>()
                .ContentTypeIs("showcase")
                .FieldEquals("fields.slug", ShowcaseSlug)
                .Include(3);

            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();
            var Showcase = _contentfulFactory.ToModel(entry);

            return Showcase.GetType() == typeof(NullHomepage) ?
                HttpResponse.Failure(HttpStatusCode.NotFound, $"No Showcase found") :
                HttpResponse.Successful(Showcase);
        }

        private string UrlFor(string type, int referenceLevel, string slug = null)
        {
            return slug == null
                ? $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}"
                : $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}";
        }
    }
}
