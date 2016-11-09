using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class StartPageRepository
    {
        private readonly IFactory<StartPage> _startPageFactory;
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;

        public StartPageRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<StartPage> startPageFactory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _startPageFactory = startPageFactory;
        }

        public async Task<HttpResponse> GetStartPage(string startPageSlug)
        {
            var contentfulResponse = await _contentfulClient.Get(UrlFor("startPage", 1, startPageSlug));

            if (!contentfulResponse.HasItems())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'");

            var startPageEntry = contentfulResponse.GetFirstItem();
            var startPage =  _startPageFactory.Build(startPageEntry, contentfulResponse);

            return startPage.GetType() == typeof(NullStartPage) ?
                HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'") : 
                HttpResponse.Successful(startPage);
        }

        private string UrlFor(string type, int referenceLevel, string slug = null)
        {
            return slug == null
                ? $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}"
                : $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}";
        }

    }
}
