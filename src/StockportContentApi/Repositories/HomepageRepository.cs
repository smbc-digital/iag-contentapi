using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class HomepageRepository
    {
        private readonly IFactory<Homepage> _homepageFactory;
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private readonly UrlBuilder _urlBuilder;

        public HomepageRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<Homepage> homepageFactory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _homepageFactory = homepageFactory;
            _urlBuilder = new UrlBuilder(_contentfulApiUrl);
        }

        public async Task<HttpResponse> Get()
        {
            var contentfulResponse = await _contentfulClient.Get(_urlBuilder.UrlFor(type:"homepage", referenceLevel:2));

            var homepageEntry = contentfulResponse.GetFirstItem();
            var homepage = _homepageFactory.Build(homepageEntry, contentfulResponse);

            return homepage.GetType() == typeof(NullHomepage) ?
                HttpResponse.Failure(HttpStatusCode.NotFound, $"No homepage found") :
                HttpResponse.Successful(homepage);
        }      
    }
}
