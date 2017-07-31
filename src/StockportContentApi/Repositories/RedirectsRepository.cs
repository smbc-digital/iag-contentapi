using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Factories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
namespace StockportContentApi.Repositories
{
    public class RedirectsRepository : BaseRepository
    {
        public IContentfulClientManager ClientManager;
        private const string ContentType = "redirect";
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly RedirectBusinessIds _redirectBusinessIds;
        private Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects> _contenfulFactory;

        public RedirectsRepository(IContentfulClientManager clientManager, Func<string, ContentfulConfig> createConfig, RedirectBusinessIds redirectBusinessIds, IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects> contenfulFactory)
        {
            _createConfig = createConfig;
            _redirectBusinessIds = redirectBusinessIds;
            ClientManager = clientManager;
            _contenfulFactory = contenfulFactory;
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

            _client = ClientManager.GetClient(config);
            var builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs(ContentType).Include(1);
            var entries = await _client.GetEntriesAsync(builder);

            return !entries.Any() 
                ? new NullBusinessIdToRedirects()
                : _contenfulFactory.ToModel(entries.FirstOrDefault());
        }
    }
}