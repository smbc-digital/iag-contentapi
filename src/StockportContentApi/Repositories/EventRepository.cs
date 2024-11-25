﻿namespace StockportContentApi.Repositories;

public class EventRepository : BaseRepository
{
    private readonly ICache _cache;
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulEventHomepage, EventHomepage> _contentfulEventHomepageFactory;
    private readonly IContentfulFactory<ContentfulEvent, Event> _contentfulFactory;
    private readonly DateComparer _dateComparer;
    private readonly int _eventsTimeout;
    private readonly IConfiguration _configuration;
    private readonly CacheKeyConfig _cacheKeyConfig;
    private readonly ITimeProvider _timeProvider;
    private readonly string _allEventsCacheKey;
    private readonly string _eventCategoriesCacheKey;

    public EventRepository(
        ContentfulConfig contentfulConfig,
        CacheKeyConfig cacheKeyConfig,
        IContentfulClientManager contentfulClientManager,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulEvent, Event> contentfulFactory,
        IContentfulFactory<ContentfulEventHomepage, EventHomepage> contentfulEventHomepageFactory,
        ICache cache,
        IConfiguration configuration)
    {
        _contentfulFactory = contentfulFactory;
        _contentfulEventHomepageFactory = contentfulEventHomepageFactory;
        _dateComparer = new(timeProvider);
        _client = contentfulClientManager.GetClient(contentfulConfig);
        _cacheKeyConfig = cacheKeyConfig;
        _cache = cache;
        _configuration = configuration;
        int.TryParse(_configuration["redisExpiryTimes:Events"], out _eventsTimeout);
        _timeProvider = timeProvider;
        _allEventsCacheKey = $"{_cacheKeyConfig.EventsCacheKey}-all";
        _eventCategoriesCacheKey = $"{_cacheKeyConfig.EventsCacheKey}-categories";
    }

    public async Task<HttpResponse> GetEventHomepage(int quantity = 3)
    {
        QueryBuilder<ContentfulEventHomepage> builder = new QueryBuilder<ContentfulEventHomepage>().ContentTypeIs("eventHomepage").Include(1);
        ContentfulCollection<ContentfulEventHomepage> entries = await _client.GetEntries(builder);
        ContentfulEventHomepage entry = entries.ToList().First();

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No event homepage found")
            : HttpResponse.Successful(await AddHomepageRowEvents(_contentfulEventHomepageFactory.ToModel(entry), quantity));
    }

    public async Task<IEnumerable<ContentfulEvent>> GetAllEventsForAGroup(string groupSlug)
    {
        IList<ContentfulEvent> events = await _cache.GetFromCacheOrDirectlyAsync(_allEventsCacheKey, GetAllEvents, _eventsTimeout);
        IEnumerable<ContentfulEvent> groupEvents = events.Where(singleEvent => singleEvent.Group.Slug.Equals(groupSlug));
        
        return groupEvents;
    }

    private async Task<EventHomepage> AddHomepageRowEvents(EventHomepage homepage, int quantity = 3)
    {
        IList<ContentfulEvent> events = await _cache.GetFromCacheOrDirectlyAsync(_allEventsCacheKey, GetAllEvents, _eventsTimeout);
        
        List<Event> liveEvents = GetAllEventsAndTheirRecurrences(events)
                                    .Where(singleEvent => _dateComparer.EventDateIsBetweenTodayAndLater(singleEvent.EventDate))
                                    .OrderBy(singleEvent => singleEvent.EventDate)
                                    .ThenBy(singleEvent => TimeSpan.Parse(singleEvent.StartTime))
                                    .ThenBy(singleEvent => singleEvent.Title)
                                    .ToList();

        liveEvents = GetNextOccurenceOfEvents(liveEvents);

        foreach (EventHomepageRow row in homepage.Rows)
            row.Events = row.IsLatest
                ? liveEvents.Take(quantity) 
                : liveEvents.Where(singleEvent => singleEvent.Tags.Contains(row.Tag.ToLower())).Take(quantity);

        return homepage;
    }

    public async Task<ContentfulCollection<ContentfulEventCategory>> GetContentfulEventCategories() =>
        await _cache.GetFromCacheOrDirectlyAsync(_eventCategoriesCacheKey, GetContentfulEventCategoriesDirect, _eventsTimeout);
        

    /// <summary>
    /// Get event categories from contentful event categories type
    /// </summary>
    /// <returns></returns>
    private async Task<ContentfulCollection<ContentfulEventCategory>> GetContentfulEventCategoriesDirect()
    {
        QueryBuilder<ContentfulEventCategory> eventCategoryBuilder = new QueryBuilder<ContentfulEventCategory>().ContentTypeIs("eventCategory").Include(1);
        ContentfulCollection<ContentfulEventCategory> result = await _client.GetEntries(eventCategoryBuilder);

        return !result.Any() 
            ? null 
            : result;
    }

    public async Task<HttpResponse> GetEvent(string slug, DateTime? date)
    {
        // does this need to retrieve all events and their occurrences??
        IList<ContentfulEvent> entries = await _cache.GetFromCacheOrDirectlyAsync(_allEventsCacheKey, GetAllEvents, _eventsTimeout);
        IEnumerable<Event> events = GetAllEventsAndTheirRecurrences(entries);
        Event eventItem = events.FirstOrDefault(singleEvent => singleEvent.Slug.Equals(slug));

        eventItem = GetEventFromItsOccurrences(date, eventItem);

        if (eventItem is not null && !string.IsNullOrEmpty(eventItem.Group?.Slug) &&
                !_dateComparer.DateNowIsNotBetweenHiddenRange(eventItem.Group.DateHiddenFrom, eventItem.Group.DateHiddenTo))
            eventItem.Group = new NullGroup();

        if (eventItem is not null)
            eventItem.RelatedEvents = GetRelatedEvents(entries,
                                                eventItem.Slug,
                                                eventItem.EventCategories.Select(cat => cat.Name).ToList(),
                                                eventItem.EventCategories.Select(cat => cat.Slug).ToList(),
                                                eventItem.Tags);

        return eventItem is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No event found for '{slug}'")
            : HttpResponse.Successful(eventItem);
    }

    private static Event GetEventFromItsOccurrences(DateTime? date, Event eventItem)
    {
        if (eventItem is null || !date.HasValue || eventItem.EventDate.Equals(date))
            return eventItem;

        return new EventRecurrenceFactory()
                    .GetRecurringEventsOfEvent(eventItem)
                    .SingleOrDefault(evnt => evnt.EventDate.Equals(date));
    }

    public async Task<HttpResponse> Get(DateTime? dateFrom, DateTime? dateTo, string category, int limit,
        bool? displayFeatured, string tag, string price, double latitude, double longitude)
    {
        IList<ContentfulEvent> entries = await _cache.GetFromCacheOrDirectlyAsync(_allEventsCacheKey, GetAllEvents, _eventsTimeout);

        if (entries is null || !entries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No events found");

        DateTime? searchdateFrom = dateFrom;
        DateTime? searchdateTo = dateTo;
        DateTime now = _timeProvider.Now().Date;

        if (!dateFrom.HasValue && !dateTo.HasValue)
        {
            searchdateFrom = now;
            searchdateTo = DateTime.MaxValue;
        }
        else if (dateFrom.HasValue && !dateTo.HasValue)
            searchdateTo = DateTime.MaxValue;
        else if (!dateFrom.HasValue && dateTo.HasValue && dateTo.Value.Date < now)
            searchdateFrom = DateTime.MinValue;
        else if (!dateFrom.HasValue && dateTo.HasValue && dateTo.Value.Date >= now)
            searchdateFrom = now;

        GeoCoordinate searchCoord = new(latitude, longitude);

        List<Event> events = GetAllEventsAndTheirRecurrences(entries)
                                .Where(e => CheckDates(searchdateFrom, searchdateTo, e))
                                .Where(e => string.IsNullOrWhiteSpace(category) 
                                    || e.EventCategories.Any(c => c.Slug.ToLower().Equals(category.ToLower())) 
                                    || e.EventCategories.Any(c => c.Name.ToLower().Equals(category.ToLower())))
                                .Where(e => string.IsNullOrWhiteSpace(tag) || e.Tags.Contains(tag.ToLower()))
                                .Where(e => string.IsNullOrWhiteSpace(price) || price.ToLower().Equals("paid,free") 
                                    || price.ToLower().Equals("free,paid") 
                                    || price.ToLower().Equals("free") && (e.Free ?? false)
                                    || price.ToLower().Equals("paid") && (e.Paid ?? false))
                                .Where(e => latitude.Equals(0) && longitude.Equals(0) || searchCoord.GetDistanceTo(e.Coord) < 3200)
                                .OrderBy(o => o.EventDate)
                                .ThenBy(c => TimeSpan.Parse(c.StartTime))
                                .ThenBy(t => t.Title)
                                .ToList();

        if (displayFeatured is not null && displayFeatured is true)
            events = events.OrderBy(e => e.Featured ? 0 : 1).ToList();

        if (limit > 0)
            events = events.Take(limit).ToList();

        var eventCategoriesCollection = await GetContentfulEventCategories();
        IEnumerable<string> eventCategories = Enumerable.Empty<string>();

        if(eventCategoriesCollection is not null)
            eventCategories = eventCategoriesCollection.Select(eventCategory => eventCategory.Name);

        EventCalender eventCalender = new();
        eventCalender.SetEvents(events, eventCategories.ToList());

        return HttpResponse.Successful(eventCalender);
    }

    public virtual async Task<List<Event>> GetEventsByCategory(string category, bool onlyNextOccurrence)
    {
        IList<ContentfulEvent> entries = await _cache.GetFromCacheOrDirectlyAsync(_allEventsCacheKey, GetAllEvents, _eventsTimeout);

        List<Event> events = GetAllEventsAndTheirRecurrences(entries).Where(e => string.IsNullOrWhiteSpace(category)
                                    || e.EventCategories.Select(c => c.Slug.ToLower()).Contains(category.ToLower())
                                    || e.EventCategories.Select(c => c.Name.ToLower()).Contains(category.ToLower()))
                                .Where(e => _dateComparer.EventDateIsBetweenTodayAndLater(e.EventDate))
                                .OrderBy(o => o.EventDate)
                                .ThenBy(c => TimeSpan.Parse(c.StartTime))
                                .ThenBy(t => t.Title)
                                .ToList();

        return onlyNextOccurrence
            ? GetNextOccurenceOfEvents(events)
            : events;
    }

    public async Task<EventCalender> GetFreeEvents()
    {
        IList<ContentfulEvent> entries = await _cache.GetFromCacheOrDirectlyAsync(_allEventsCacheKey, GetAllEvents, _eventsTimeout);

        List<Event> freeEvents = GetAllEventsAndTheirRecurrences(entries)
            .Where(e => _dateComparer.EventDateIsBetweenTodayAndLater(e.EventDate))
            .Where(e => e.Free is not null && e.Free is true)
            .OrderBy(o => o.EventDate)
            .ThenBy(c => TimeSpan.Parse(c.StartTime))
            .ThenBy(t => t.Title)
            .ToList();

        EventCalender eventCalender = new();
        eventCalender.SetEvents(freeEvents, new List<string>());

        return eventCalender;
    }

    public virtual async Task<List<Event>> GetEventsByTag(string tag, bool onlyNextOccurrence)
    {
        IList<ContentfulEvent> entries = await _cache.GetFromCacheOrDirectlyAsync(_allEventsCacheKey, GetAllEvents, _eventsTimeout);

        List<Event> events = GetAllEventsAndTheirRecurrences(entries).Where(e => string.IsNullOrWhiteSpace(tag)
                                    || e.Tags.Contains(tag.ToLower()))
                                .Where(e => _dateComparer.EventDateIsBetweenTodayAndLater(e.EventDate))
                                .OrderBy(o => o.EventDate)
                                .ThenBy(c => TimeSpan.Parse(c.StartTime))
                                .ThenBy(t => t.Title)
                                .ToList();

        return onlyNextOccurrence
            ? GetNextOccurenceOfEvents(events)
            : events;
    }

    private static List<Event> GetNextOccurenceOfEvents(List<Event> events)
    {
        List<Event> result = new();

        foreach (Event item in events)
        {
            if (!result.Any(i => i.Slug.Equals(item.Slug)))
                result.Add(item);
        }

        return result;
    }

    public async Task<IList<ContentfulEvent>> GetAllEvents()
    {
        QueryBuilder<ContentfulEvent> builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").Include(2);
        ContentfulCollection<ContentfulEvent> entries = await GetAllEntriesAsync(_client, builder);
        IEnumerable<ContentfulEvent> publishedEvents = entries.Where(e => _dateComparer.DateNowIsNotBetweenHiddenRange(e.Group.DateHiddenFrom, e.Group.DateHiddenTo));
        
        return !publishedEvents.Any()
            ? null 
            : publishedEvents.ToList();
    }

    public IEnumerable<Event> GetAllEventsAndTheirRecurrences(IEnumerable<ContentfulEvent> entries)
    {
        List<Event> entriesList = new();

        foreach (ContentfulEvent entry in entries)
        {
            Event eventItem = _contentfulFactory.ToModel(entry);
            entriesList.Add(eventItem);
            entriesList.AddRange(new EventRecurrenceFactory().GetRecurringEventsOfEvent(eventItem));
        }

        return entriesList;
    }

    private bool CheckDates(DateTime? startDate, DateTime? endDate, Event events) =>
        startDate.HasValue && endDate.HasValue
            ? _dateComparer.EventDateIsBetweenStartAndEndDates(events.EventDate, startDate.Value, endDate.Value)
            : _dateComparer.EventDateIsBetweenTodayAndLater(events.EventDate);

    public async Task<List<Event>> GetLinkedEvents<T>(string slug)
    {
        IList<ContentfulEvent> entries = await _cache.GetFromCacheOrDirectlyAsync(_allEventsCacheKey, GetAllEvents, _eventsTimeout);

        List<Event> events = GetAllEventsAndTheirRecurrences(entries).Where(e => e.Group.Slug.Equals(slug))
                                .Where(e => _dateComparer.EventDateIsBetweenTodayAndLater(e.EventDate))
                                .OrderBy(o => o.EventDate)
                                .ThenBy(c => TimeSpan.Parse(c.StartTime))
                                .ThenBy(t => t.Title)
                                .ToList();

        return GetNextOccurenceOfEvents(events);
    }

    public List<Event> GetRelatedEvents(IList<ContentfulEvent> entries, string slug, List<string> categoryNames, List<string> categorySlugs, List<string> tags)
    {
        List<Event> events = GetAllEventsAndTheirRecurrences(entries)
            .Where(evnt => !evnt.Slug.Equals(slug))
            .Where(evnt =>
                evnt.EventCategories.Any(cat => categorySlugs.Contains(cat.Slug, StringComparer.OrdinalIgnoreCase) ||
                                                categoryNames.Contains(cat.Name, StringComparer.OrdinalIgnoreCase)) ||
                evnt.Tags.Any(tag => tags.Contains(tag, StringComparer.OrdinalIgnoreCase)))
            .Where(evnt => _dateComparer.EventDateIsBetweenTodayAndLater(evnt.EventDate))
            .OrderBy(evnt => evnt.EventDate)
            .ThenBy(evnt => TimeSpan.Parse(evnt.StartTime))
            .ToList();

        return GetNextOccurenceOfEvents(events);
    }

    public async Task<ContentfulEvent> GetContentfulEvent(string slug)
    {
        QueryBuilder<ContentfulEvent> builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(1);
        ContentfulCollection<ContentfulEvent> entries = await _client.GetEntries(builder);
        ContentfulEvent entry = entries.FirstOrDefault();

        return entry;
    }
}