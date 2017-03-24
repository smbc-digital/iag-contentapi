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
    public class SubhomepageRepository
    {
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private readonly IContentfulFactory<Entry<ContentfulSubhomepage>, Subhomepage> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public SubhomepageRepository(ContentfulConfig config, IHttpClient httpClient, IContentfulFactory<Entry<ContentfulSubhomepage>, Subhomepage> subhhomepageBuilder, IContentfulClientManager contentfulClientManager)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _contentfulFactory = subhhomepageBuilder;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetSubhomepages(string SubhomepageSlug)
        {
            var builder = new QueryBuilder<Entry<ContentfulSubhomepage>>()
                .ContentTypeIs("subhomepage")
                .FieldEquals("fields.slug", SubhomepageSlug)
                .Include(3);

            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();
            var Subhomepage = _contentfulFactory.ToModel(entry);

            return Subhomepage.GetType() == typeof(NullHomepage) ?
                HttpResponse.Failure(HttpStatusCode.NotFound, $"No Subhomepage found") :
                HttpResponse.Successful(Subhomepage);
        }

        private string UrlFor(string type, int referenceLevel, string slug = null)
        {
            return slug == null
                ? $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}"
                : $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}";
        }
    }
}
