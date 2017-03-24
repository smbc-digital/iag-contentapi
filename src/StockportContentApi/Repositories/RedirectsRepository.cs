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
    public class RedirectsRepository
    {
        private readonly ContentfulClient _contentfulClient;
        private const string ContentType = "redirect";
        private readonly IFactory<BusinessIdToRedirects> _factory;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly RedirectBusinessIds _redirectBusinessIds;
        private  UrlBuilder _urlBuilder;
        public RedirectsRepository(IHttpClient httpClient, IFactory<BusinessIdToRedirects> factory, Func<string, ContentfulConfig> createConfig, RedirectBusinessIds redirectBusinessIds)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _factory = factory;
            _createConfig = createConfig;
            _redirectBusinessIds = redirectBusinessIds;
            
        }

        public async Task<HttpResponse> GetRedirects()
        {
            var redirectPerBusinessId = new Dictionary<string, BusinessIdToRedirects>();

            foreach (var businessId in _redirectBusinessIds.BusinessIds)
            {
                redirectPerBusinessId.Add(businessId, await GetRedirectForBusinessId(businessId));
            }

            return !redirectPerBusinessId.Any() 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "Redirects not found") 
                : HttpResponse.Successful(GetRedirectsFromBusinessIdToRedirectsDictionary(redirectPerBusinessId));
        }

        private Redirects GetRedirectsFromBusinessIdToRedirectsDictionary(Dictionary<string, BusinessIdToRedirects> redirects)
        {
            var shortUrlRedirects = new Dictionary<string, RedirectDictionary>();
            var legacyUrlRedirects = new Dictionary<string, RedirectDictionary>();

            foreach (var businessId in redirects.Keys)
            {
                shortUrlRedirects.Add(businessId, redirects[businessId].ShortUrlRedirects);
                legacyUrlRedirects.Add(businessId, redirects[businessId].LegacyUrlRedirects);
            }

            return new Redirects(shortUrlRedirects, legacyUrlRedirects);
        } 

        private async Task<BusinessIdToRedirects> GetRedirectForBusinessId(string businessId)
        {
            var config = _createConfig(businessId);
            _urlBuilder = new UrlBuilder(config.ContentfulUrl.ToString());
            var contentfulResponse = await _contentfulClient.Get(_urlBuilder.UrlFor(ContentType));

            return !contentfulResponse.HasItems() 
                ? new NullBusinessIdToRedirects() 
                : _factory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);
        }
    }
}