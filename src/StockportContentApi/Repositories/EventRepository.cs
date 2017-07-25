using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using Microsoft.Extensions.Logging;
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
    public class EventRepository : BaseRepository
    {
        private readonly IContentfulFactory<ContentfulEvent, Event> _contentfulFactory;
        private readonly IContentfulFactory<ContentfulEventHomepage, EventHomepage> _contentfulEventHomepageFactory;
        private readonly DateComparer _dateComparer;
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly ICache _cache;
        private ILogger<EventRepository> _logger;
        private ITimeProvider _timeProvider;

        public EventRepository(ContentfulConfig config,
            IContentfulClientManager contentfulClientManager, ITimeProvider timeProvider, 
            IContentfulFactory<ContentfulEvent, Event> contentfulFactory,
            IContentfulFactory<ContentfulEventHomepage, EventHomepage> contentfulEventHomepageFactory,
            ICache cache,
            ILogger<EventRepository> logger
            )
        {
            _contentfulFactory = contentfulFactory;
            _contentfulEventHomepageFactory = contentfulEventHomepageFactory;
            _dateComparer = new DateComparer(timeProvider);
            _client = contentfulClientManager.GetClient(config);
            _cache = cache;
            _logger = logger;
            _timeProvider = timeProvider;
        }

        public async Task<HttpResponse> GetEventHomepage()
        {
            var builder = new QueryBuilder<ContentfulEventHomepage>().ContentTypeIs("eventHomepage").Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.ToList().First();
            
            return entry == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No event homepage found")
                : HttpResponse.Successful(await AddHomepageRowEvents(_contentfulEventHomepageFactory.ToModel(entry)));
        }

        private async Task<EventHomepage> AddHomepageRowEvents(EventHomepage homepage)
        {
            var events = await _cache.GetFromCacheOrDirectlyAsync("event-all", GetAllEvents);
            var liveEvents = GetAllEventsAndTheirReccurrences(events).Where(e => _dateComparer.EventDateIsBetweenTodayAndLater(e.EventDate)).OrderBy(e => e.EventDate).ToList();
            liveEvents = GetNextOccurenceOfEvents(liveEvents);

            foreach (var row in homepage.Rows)
            {
                if (row.IsLatest)
                {
                    row.Events = liveEvents.Take(4);
                }
                else
                {
                    row.Events = liveEvents.Where(e => e.Tags.Contains(row.Tag.ToLower())).Take(4);
                }
            }

            return homepage;
        }

        public async Task<HttpResponse> GetEvent(string slug, DateTime? date)
        {
            var entries = await _cache.GetFromCacheOrDirectlyAsync("event-all", GetAllEvents);

            var events = GetAllEventsAndTheirReccurrences(entries);

            var eventItem = events.Where(e => e.Slug == slug).FirstOrDefault();

            eventItem = GetEventFromItsOccurrences(date, eventItem);
            if (eventItem != null && !string.IsNullOrEmpty(eventItem.Group?.Slug) && !_dateComparer.DateNowIsNotBetweenHiddenRange(eventItem.Group.DateHiddenFrom, eventItem.Group.DateHiddenTo))
            {
                eventItem.Group = new Group(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new List<GroupCategory>(), new List<Crumb>(), new MapPosition(), false, null, null, null, "published", string.Empty, string.Empty, string.Empty);
            }

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

            var searchdateFrom = dateFrom;
            var searchdateTo = dateTo;

            var now = _timeProvider.Now().Date;

            if (!dateFrom.HasValue && !dateTo.HasValue)
            {
                searchdateFrom = now;
                searchdateTo = DateTime.MaxValue;
            }
            else if (dateFrom.HasValue && !dateTo.HasValue)
            {
                searchdateTo = DateTime.MaxValue;
            }
            else if (!dateFrom.HasValue && dateTo.HasValue && dateTo.Value.Date < now)
            {
                searchdateFrom = DateTime.MinValue;
            }
            else if (!dateFrom.HasValue && dateTo.HasValue && dateTo.Value.Date >= now)
            {
                searchdateFrom = now;
            }

            var events =
                    GetAllEventsAndTheirReccurrences(entries)
                    .Where(e => CheckDates(searchdateFrom, searchdateTo, e))
                    .Where(e => string.IsNullOrWhiteSpace(category) || e.Categories.Contains(category.ToLower()) || e.EventCategories.Any(c => c.Slug == category.ToLower()))
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
            var entries = await _cache.GetFromCacheOrDirectlyAsync("event-all", GetAllEvents);

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
            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").Include(2);
            var entries = await GetAllEntriesAsync(_client, builder);
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
            var entries = await _cache.GetFromCacheOrDirectlyAsync("event-all", GetAllEvents);

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
            return await _cache.GetFromCacheOrDirectlyAsync("event-categories", GetCategoriesDirect);
        }

        private async Task<List<string>> GetCategoriesDirect()
        {
            var eventType = await _client.GetContentTypeAsync("events");
            var validation = eventType.Fields.First(f => f.Name == "Categories").Items.Validations[0] as Contentful.Core.Models.Management.InValuesValidator;
            return validation.RequiredValues;
        }

        public async Task<ContentfulEvent> GetContentfulEvent(string slug)
        {
            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            return entry;
        }
    }
}
