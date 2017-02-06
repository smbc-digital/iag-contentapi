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
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class TopicRepository
    {
        private readonly IContentfulFactory<ContentfulTopic, Topic> _topicFactory;
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly ITimeProvider _timeProvider;

        public TopicRepository(ContentfulConfig config, IContentfulClientManager clientManager, 
                               IContentfulFactory<ContentfulTopic, Topic> topicFactory, ITimeProvider timeProvider)
        {
            _client = clientManager.GetClient(config);
            _topicFactory = topicFactory;
            _timeProvider = timeProvider;
        }

        public async Task<HttpResponse> GetTopicByTopicSlug(string slug)
        {
            var builder = new QueryBuilder().ContentTypeIs("topic").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync<ContentfulTopic>(builder);
            var entry = entries.FirstOrDefault();

            if (entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No topic found for '{slug}'");

            var model = _topicFactory.ToModel(entry);

            // filter alerts
            var alertsFiltered = model.Alerts.Where(a => a.SunriseDate <= _timeProvider.Now() && a.SunsetDate >= _timeProvider.Now()).ToList();

            model.SetAlerts(alertsFiltered);
            
            return HttpResponse.Successful(model);
        }
    }
}