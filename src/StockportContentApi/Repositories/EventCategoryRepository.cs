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
using System.Collections.Generic;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class EventCategoryRepository
    {
        private readonly IContentfulFactory<ContentfulEventCategory, EventCategory> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly ICache _cache;

        public EventCategoryRepository(ContentfulConfig config, IContentfulFactory<ContentfulEventCategory, EventCategory> contentfulFactory, IContentfulClientManager contentfulClientManager, ICache cache)
        {
            _contentfulFactory = contentfulFactory;
            _client = contentfulClientManager.GetClient(config);
            _cache = cache;
        }

        public async Task<HttpResponse> GetEventCategories()
        {
            var categories = await _cache.GetFromCacheOrDirectlyAsync("event-categories-content-type", GetCategoriesDirect);

            if(categories != null && !categories.Any())
            {
                return HttpResponse.Failure(HttpStatusCode.NotFound, "No categories returned");
            }

            categories = categories.OrderBy(c => c.Name).ToList();
            return HttpResponse.Successful(categories);
        }

        private async Task<List<EventCategory>> GetCategoriesDirect()
        {
            var builder = new QueryBuilder<ContentfulEventCategory>().ContentTypeIs("eventCategory");

            var entries = await _client.GetEntriesAsync(builder);
            if (entries == null || !entries.Any()) new List<EventCategory>();

            var eventCategories = entries.Select(eventCatogory => _contentfulFactory.ToModel(eventCatogory)).ToList();

            return eventCategories;
        }
    }
}
