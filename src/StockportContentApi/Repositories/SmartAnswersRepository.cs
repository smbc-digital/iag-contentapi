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

namespace StockportContentApi.Repositories
{
    public class SmartAnswersRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulSmartAnswers, SmartAnswer> _contentfulFactory;

        public SmartAnswersRepository(ContentfulConfig config, IContentfulClientManager contentfulClientManager, IContentfulFactory<ContentfulSmartAnswers, SmartAnswer> contentfulFactory)
        {
            _client = contentfulClientManager.GetClient(config);
            _contentfulFactory = contentfulFactory;
        }

        public async Task<HttpResponse> Get(string slug)
        {
            var builder = new QueryBuilder<ContentfulSmartAnswers>().ContentTypeIs("smartAnswers").FieldEquals("fields.slug",slug).Include(1);
            var entires = await _client.GetEntriesAsync(builder);

            if (entires == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No smart answer found for '{slug}'");

            var entry = entires.FirstOrDefault();

            var smartAnswer = _contentfulFactory.ToModel(entry);

            return HttpResponse.Successful(smartAnswer);
        }
    }
}
