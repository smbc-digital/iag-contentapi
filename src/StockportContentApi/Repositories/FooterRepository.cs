using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Config;
using StockportContentApi.Factories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class FooterRepository
    {
        private readonly string _contentfulApiUrl;
        private readonly string _contentType = "footer";
        private readonly string _businessId;
        private readonly IFactory<Footer> _factory;
        private readonly ContentfulClient _contentfulClient;
        private readonly UrlBuilder _urlBuilder;

        public FooterRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<Footer> factory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _factory = factory;
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _businessId = config.BusinessId;
            _urlBuilder = new UrlBuilder(_contentfulApiUrl);
        }

        public async Task<HttpResponse> GetFooter() {
            var referenceLevelLimit = 1;
            var contentfulResponse = await _contentfulClient.Get(_urlBuilder.UrlFor(type:_contentType, referenceLevel:referenceLevelLimit));

            if (!contentfulResponse.HasItems())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No footer found for '{_businessId}'");

            var footer = _factory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            return footer == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No footer found for '{_businessId}'")
                : HttpResponse.Successful(footer);
        }       
    }
}
