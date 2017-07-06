using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Contentful.Core.Search;

namespace StockportContentApi.Repositories
{
    public class StartPageRepository
    {
        private readonly IFactory<StartPage> _startPageFactory;
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private readonly UrlBuilder _urlBuilder;

        public StartPageRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<StartPage> startPageFactory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _startPageFactory = startPageFactory;
            _urlBuilder = new UrlBuilder(_contentfulApiUrl);
        }

        public async Task<HttpResponse> GetStartPage(string startPageSlug)
        {
            var contentfulResponse = await _contentfulClient.Get(_urlBuilder.UrlFor(type:"startPage", referenceLevel:1, slug:startPageSlug));

            if (!contentfulResponse.HasItems())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'");

            var startPageEntry = contentfulResponse.GetFirstItem();
            var startPage =  _startPageFactory.Build(startPageEntry, contentfulResponse);

            return startPage.GetType() == typeof(NullStartPage) ?
                HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'") : 
                HttpResponse.Successful(startPage);
        }

        public async Task<HttpResponse> Get()
        {
            //var builder = new QueryBuilder<ContentfulStartPage>().ContentTypeIs("startPage").Include(2);
            //var entries = await _client.GetEntriesAsync(builder);

            //if (entries == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No topics found");

            //var models = entries.Select(e => _topicFactory.ToModel(e));

            //return HttpResponse.Successful(models);
            return null;
        }
    }
}
