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
    public class EventCategoryRepository
    {
        private readonly IContentfulFactory<ContentfulEventCategory, EventCategory> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public EventCategoryRepository(ContentfulConfig config, IContentfulFactory<ContentfulEventCategory, EventCategory> contentfulFactory, IContentfulClientManager contentfulClientManager)
        {
            _contentfulFactory = contentfulFactory;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetEventCategories()
        {
            var builder = new QueryBuilder<ContentfulEventCategory>().ContentTypeIs("eventCategory");

            var entries = await _client.GetEntriesAsync(builder);
            if (entries == null || !entries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No event catogories found");

            var eventCategories = entries.Select(eventCatogory => _contentfulFactory.ToModel(eventCatogory)).ToList();

            return HttpResponse.Successful(eventCategories);
        }
    }
}
