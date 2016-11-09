using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class HomepageRepository
    {
        private readonly IFactory<Homepage> _homepageFactory;
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;

        public HomepageRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<Homepage> homepageFactory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _homepageFactory = homepageFactory;
        }

        public async Task<HttpResponse> Get()
        {
            var contentfulResponse = await _contentfulClient.Get(UrlFor("homepage", 2));

            var homepageEntry = contentfulResponse.GetFirstItem();
            var homepage = _homepageFactory.Build(homepageEntry, contentfulResponse);

            return homepage.GetType() == typeof(NullHomepage) ?
                HttpResponse.Failure(HttpStatusCode.NotFound, $"No homepage found") :
                HttpResponse.Successful(homepage);
        }

        private string UrlFor(string type, int referenceLevel, string slug = null)
        {
            return slug == null
                ? $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}"
                : $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}";
        }
    }
}
