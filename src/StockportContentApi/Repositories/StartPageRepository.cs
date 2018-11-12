using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;

namespace StockportContentApi.Repositories
{
    public class StartPageRepository
    {
        private readonly IContentfulFactory<ContentfulStartPage, StartPage> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public StartPageRepository(ContentfulConfig config, IContentfulClientManager contentfulClientManager, IContentfulFactory<ContentfulStartPage, StartPage> contentfulFactory)
        {
            _contentfulFactory = contentfulFactory;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetStartPage(string startPageSlug)
        {
            var builder = new QueryBuilder<ContentfulStartPage>()
                .ContentTypeIs("startPage")
                .FieldEquals("fields.slug", startPageSlug)
                .Include(3);

            var entries = await _client.GetEntries(builder);

            if (!entries.Any())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'");

            var startPageEntry = entries.FirstOrDefault();
            var startPage = _contentfulFactory.ToModel(startPageEntry);

            return startPage.GetType() == typeof(NullStartPage) ?
                HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'") : 
                HttpResponse.Successful(startPage);
        }

        public async Task<HttpResponse> Get()
        {
            var builder = new QueryBuilder<ContentfulStartPage>()
                .ContentTypeIs("startPage")
                .Include(3);

            var entries = await _client.GetEntries(builder);

            if (!entries.Any())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found");

            var startPages = entries.Select(s => _contentfulFactory.ToModel(s)).ToList();
          
            return startPages == null || !startPages.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Topics found")
                : HttpResponse.Successful(startPages);
        }
    }
}
