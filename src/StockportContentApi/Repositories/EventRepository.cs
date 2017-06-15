using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Factories;
using StockportContentApi.Helpers;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class EventRepository
    {
        private readonly IContentfulFactory<ContentfulEvent, Event> _contentfulFactory;
        private readonly DateComparer _dateComparer;
        private readonly IEventCategoriesFactory _eventCategoriesFactory;
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly string _contentfulContentTypesUrl;
        private readonly IContentfulClient _contentfulClient;
        private readonly ICacheWrapper _cache;
        private ILogger<EventRepository> _logger;

        public EventRepository(ContentfulConfig config,
            IHttpClient httpClient,
            IContentfulClientManager contentfulClientManager, ITimeProvider timeProvider, 
            IContentfulFactory<ContentfulEvent, Event> contentfulFactory,
            IEventCategoriesFactory eventCategoriesFactory,
            ICacheWrapper cache,
            ILogger<EventRepository> logger
            )
        {
            _contentfulFactory = contentfulFactory;
            _dateComparer = new DateComparer(timeProvider);
            _client = contentfulClientManager.GetClient(config);
            _eventCategoriesFactory = eventCategoriesFactory;
            _contentfulContentTypesUrl = config.ContentfulContentTypesUrl.ToString();
            _contentfulClient = new ContentfulClient(httpClient);
            _cache = cache;
            _logger = logger;
        }

        public async Task<HttpResponse> GetEvent(string slug, DateTime? date)
        {
            var entries = await _cache.GetFromCacheOrDirectly("event-all", GetAllEvents);

            var events = GetAllEventsAndTheirReccurrences(entries);

            var eventItem = events.Where(e => e.Slug == slug).FirstOrDefault();

            eventItem = GetEventFromItsOccurrences(date, eventItem);

            return eventItem == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No event found for '{slug}'")
                : HttpResponse.Successful(eventItem);
        }

        private static Event GetEventFromItsOccurrences(DateTime? date, Event eventItem)
        {
            if (eventItem == null || !date.HasValue || eventItem.EventDate == date) return eventItem;

            return new EventReccurenceFactory()
                .GetReccuringEventsOfEvent(eventItem)
                .SingleOrDefault(x => x.EventDate == date);
        }

        public async Task<HttpResponse> Get(DateTime? dateFrom, DateTime? dateTo, string category, int limit, bool? displayFeatured, string tag)
        {
            var entries = await _cache.GetFromCacheOrDirectlyAsync("event-all", GetAllEvents);

            if (entries == null || !entries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No events found");

            var events =
                    GetAllEventsAndTheirReccurrences(entries)
                    .Where(e => CheckDates(dateFrom, dateTo, e))
                    .Where(e => string.IsNullOrWhiteSpace(category) || e.Categories.Contains(category.ToLower()))
                    .Where(e => string.IsNullOrWhiteSpace(tag) || e.Tags.Contains(tag.ToLower()))
                    .OrderBy(o => o.EventDate)
                    .ThenBy(c => c.StartTime)
                    .ThenBy(t => t.Title)
                    .ToList();

            if (displayFeatured != null && displayFeatured == true)
            {
                events = events.OrderBy(e => e.Featured ? 0 : 1).ToList();
            }

            if (limit > 0) events = events.Take(limit).ToList();
           
            var eventCategories = await GetCategories();

            var eventCalender = new EventCalender();
            eventCalender.SetEvents(events, eventCategories);

            return HttpResponse.Successful(eventCalender);
        }

        public async Task<List<Event>> GetEventsByCategory(string category)
        {
            var entries = await _cache.GetFromCacheOrDirectly("event-all", GetAllEvents);

            var events = 
                    GetAllEventsAndTheirReccurrences(entries)
                    .Where(e => string.IsNullOrWhiteSpace(category) || e.Categories.Contains(category.ToLower()))
                    .Where(e => _dateComparer.EventDateIsBetweenTodayAndLater(e.EventDate))
                    .OrderBy(o => o.EventDate)
                    .ThenBy(c => c.StartTime)
                    .ThenBy(t => t.Title)
                    .ToList();

            return GetNextOccurenceOfEvents(events);
        }

        private List<Event> GetNextOccurenceOfEvents(List<Event> events)
        {
            var result = new List<Event>();
            foreach (var item in events)
            {
                if (!result.Any(i => i.Slug == item.Slug))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private async Task<IList<ContentfulEvent>> GetAllEvents()
        {
            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").Include(2).Limit(ContentfulQueryValues.LIMIT_MAX);
            var entries = await _client.GetEntriesAsync(builder);
            return entries.ToList();
        }

        public IEnumerable<Event> GetAllEventsAndTheirReccurrences(IEnumerable<ContentfulEvent> entries)
        {
            var entriesList = new List<Event>();
            foreach (var entry in entries)
            {
                var eventItem = _contentfulFactory.ToModel(entry);
                entriesList.Add(eventItem);
                entriesList.AddRange(new EventReccurenceFactory().GetReccuringEventsOfEvent(eventItem));
            }

            return entriesList;
        }

        private bool CheckDates(DateTime? startDate, DateTime? endDate, Event events)
        {
            return startDate.HasValue && endDate.HasValue
                ? _dateComparer.EventDateIsBetweenStartAndEndDates(events.EventDate, startDate.Value, endDate.Value)
                : _dateComparer.EventDateIsBetweenTodayAndLater(events.EventDate);
        }

        public async Task<List<Event>> GetLinkedEvents<T>(string slug)
        {
            var entries = await _cache.GetFromCacheOrDirectly("event-all", GetAllEvents);

            var events = GetAllEventsAndTheirReccurrences(entries)
                    .Where(e => e.Group.Slug == slug)
                    .Where(e => _dateComparer.EventDateIsBetweenTodayAndLater(e.EventDate))
                    .OrderBy(o => o.EventDate)
                    .ThenBy(c => c.StartTime)
                    .ThenBy(t => t.Title)
                    .ToList();

            return GetNextOccurenceOfEvents(events);
        }

        public async Task<List<string>> GetCategories()
        {
            return await _cache.GetFromCacheOrDirectly("event-categories", GetCategoriesDirect);
        }

        private async Task<List<string>> GetCategoriesDirect()
        {
            var contentfulResponse = await _contentfulClient.Get(_contentfulContentTypesUrl);
            var contentfulData = contentfulResponse.Items;
            var result = _eventCategoriesFactory.Build(contentfulData);
            return result;
        }
    }
}
