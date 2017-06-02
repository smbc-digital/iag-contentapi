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
using System.Collections.Generic;

namespace StockportContentApi.Repositories
{
    public class ShowcaseRepository
    {
        private readonly IContentfulFactory<ContentfulShowcase, Showcase> _contentfulFactory;
        private readonly IContentfulFactory<List<ContentfulEvent>, List<Event>> _eventListFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public ShowcaseRepository(ContentfulConfig config, IContentfulFactory<ContentfulShowcase, Showcase> showcaseBuilder, IContentfulClientManager contentfulClientManager, IContentfulFactory<List<ContentfulEvent>, List<Event>> eventListBuilder)
        {
            _contentfulFactory = showcaseBuilder;
            _eventListFactory = eventListBuilder;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetShowcases(string slug)
        {
            var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);

            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();
            var showcase = _contentfulFactory.ToModel(entry);

            var eventbuilder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").Include(2).Limit(ContentfulQueryValues.LIMIT_MAX);
            var eventsEntry = await _client.GetEntriesAsync(eventbuilder);

            var contentfulEvents = eventsEntry.Where(e => e.Categories.Any(c => c.ToLower() == showcase.EventCategory.ToLower()))
                    .OrderBy(o => o.EventDate)
                    .ThenBy(c => c.StartTime)
                    .ThenBy(t => t.Title).Take(3)
                    .ToList();

            var events = _eventListFactory.ToModel(contentfulEvents);
            showcase.Events = events;

            return showcase.GetType() == typeof(NullHomepage) 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Showcase found") 
                : HttpResponse.Successful(showcase);
        }
    }
}
