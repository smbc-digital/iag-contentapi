using StockportContentApi.Config;
using StockportContentApi.Model;
using System.Threading.Tasks;
using StockportContentApi.Utils;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Client;
using Contentful.Core.Search;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace StockportContentApi.Repositories
{
    public interface IApiKeyRepository
    {
        Task<IEnumerable<ApiKey>> Get();
    }

    public class ApiKeyRepository : IApiKeyRepository
    {        
        private readonly IContentfulFactory<ContentfulApiKey, ApiKey> _contentfulFactory;
        private readonly int _apiKeyTimeout;
        private readonly ICache _cache;
        private readonly Contentful.Core.IContentfulClient _client;

        public ApiKeyRepository(ContentfulConfig config,
            IContentfulClientManager contentfulClientManager,
            IContentfulFactory<ContentfulApiKey, ApiKey> contentfulFactory,
            IConfiguration configuration, ICache cache)
        {
            _contentfulFactory = contentfulFactory;
            _cache = cache;
            _client = contentfulClientManager.GetClient(config);
            if (!int.TryParse(configuration["redisExpiryTimes:ApiKeys"], out _apiKeyTimeout))
            {
                _apiKeyTimeout = 60;
            };
        }
        
        public async Task<IEnumerable<ApiKey>> Get()
        {
            var entries = await _cache.GetFromCacheOrDirectlyAsync("api-keys", GetAllApiKeys, _apiKeyTimeout);

            return entries;
        }

        private async Task<IEnumerable<ApiKey>> GetAllApiKeys()
        {
            var builder = new QueryBuilder<ContentfulApiKey>().ContentTypeIs("apiKey");
            var entries = await _client.GetEntries<ContentfulApiKey>(builder);
            var contentfuApiKeys = entries as IEnumerable<ContentfulApiKey> ?? entries.ToList();

            var apiKeys = contentfuApiKeys.Select(k => _contentfulFactory.ToModel(k)).ToList();

            return entries == null || !apiKeys.Any()
                ? null
                : apiKeys;
        }
    }
   
}
