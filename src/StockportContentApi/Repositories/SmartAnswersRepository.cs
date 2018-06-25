using System;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Model;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class SmartAnswersRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulSmartAnswers, SmartAnswer> _contentfulFactory;
        private readonly ICache _cache;
        private readonly ILogger<SmartAnswersRepository> _logger;
        private IConfiguration _configuration;
        private readonly int _smartAnswersTimeout;

        public SmartAnswersRepository(ContentfulConfig config, IContentfulClientManager contentfulClientManager, IContentfulFactory<ContentfulSmartAnswers, SmartAnswer> contentfulFactory, ICache cache, ILogger<SmartAnswersRepository> logger, IConfiguration configuration)
        {
            _client = contentfulClientManager.GetClient(config);
            _contentfulFactory = contentfulFactory;
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
            int.TryParse(_configuration["redisExpiryTimes:SmartAnswers"], out _smartAnswersTimeout);
        }

        public async Task<HttpResponse> Get(string slug)
        {
            var entry = await _cache.GetFromCacheOrDirectlyAsync("smart-" + slug, () => GetSmartEntry(slug), _smartAnswersTimeout);

            if(entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "Smart not found");

            var smartAnswer = _contentfulFactory.ToModel(entry);

            return HttpResponse.Successful(smartAnswer);
        }

        private async Task<ContentfulSmartAnswers> GetSmartEntry(string slug)
        {
            var builder = new QueryBuilder<ContentfulSmartAnswers>().ContentTypeIs("smartAnswers").FieldEquals("fields.slug", slug).Include(1);
            var entires = await _client.GetEntries(builder);

            var entry = entires.FirstOrDefault();

            return entry;
        }
    }
}
