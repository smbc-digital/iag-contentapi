using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Config;
using StockportContentApi.Factories;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class FooterRepository
    {
        private readonly string _contentfulApiUrl;
        private readonly string _contentType = "footer";
        private readonly string _businessId;
        private readonly IFactory<Footer> _factory;
        private readonly ContentfulClient _contentfulClient;

        public FooterRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<Footer> factory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _factory = factory;
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _businessId = config.BusinessId;
        }

        public async Task<HttpResponse> GetFooter() {
            var referenceLevelLimit = 1;
            var contentfulResponse = await _contentfulClient.Get(UrlFor(_contentType, referenceLevelLimit));

            if (!contentfulResponse.HasItems())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No footer found for '{_businessId}'");

            var footer = _factory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            return footer == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No footer found for '{_businessId}'")
                : HttpResponse.Successful(footer);
        }

        //TODO: extract out to its own class ContentfulUrlBuilder [Tech-time]
        // + single responsibility for building urls for contentful
        // + easier to test it out and use it in the test
        // + single source of truth for building contentful urls and query
        // + one place to change the url and query
        private string UrlFor(string type, int referenceLevel)
        {
            return $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}";
        }
    }
}
