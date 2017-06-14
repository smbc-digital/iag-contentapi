using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class TopicRepository
    {
        private readonly IContentfulFactory<ContentfulTopic, Topic> _topicFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public TopicRepository(ContentfulConfig config, IContentfulClientManager clientManager, 
                               IContentfulFactory<ContentfulTopic, Topic> topicFactory)
        {
            _client = clientManager.GetClient(config);
            _topicFactory = topicFactory;
        }

        public async Task<HttpResponse> GetTopicByTopicSlug(string slug)
        {
            var builder = new QueryBuilder<ContentfulTopic>().ContentTypeIs("topic").FieldEquals("fields.slug", slug).Include(2);
            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();

            if (entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No topic found for '{slug}'");

            var model = _topicFactory.ToModel(entry);

            return HttpResponse.Successful(model);
        }
    }
}