using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class EventRepository
    {
        private readonly DateComparer _dateComparer;
        private readonly Contentful.Core.IContentfulClient _client;

        public EventRepository(ContentfulConfig config, IContentfulClientManager contentfulClientManager, ITimeProvider timeProvider)
        {
            _dateComparer = new DateComparer(timeProvider);
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetEvent(string slug, DateTime? date)
        {
            var builder = new QueryBuilder().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync<ContentfulEvent>(builder);
            var entry = entries.FirstOrDefault();

            Event eventItem = null;

            var eventsList = new List<Event>();
            if (entry != null && date.HasValue && entry.EventDate != date)
            {
                eventItem = entry.ToModel();
                eventsList.AddRange(new EventReccurenceFactory().GetReccuringEventsOfEvent(eventItem));
                eventItem = eventsList.SingleOrDefault(x => x.EventDate == date);
            }
            else if (entry != null)
            {
                eventItem = entry.ToModel();
            }

            return (entry == null || eventItem == null) 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No event found for '{slug}'") 
                : HttpResponse.Successful(eventItem);
        }

        public async Task<HttpResponse> Get(DateTime? dateFrom, DateTime? dateTo)
        {
            var builder = new QueryBuilder().ContentTypeIs("events").Include(1).Limit(ContentfulQueryValues.LIMIT_MAX);
            var entries = await _client.GetEntriesAsync<ContentfulEvent>(builder);

            if (entries == null || !entries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No events found");

            var entriesList = new List<Event>();
            foreach (var entry in entries)
            {
                var eventItem = entry.ToModel();
                entriesList.Add(eventItem);
                entriesList.AddRange(new EventReccurenceFactory().GetReccuringEventsOfEvent(eventItem));              
            }

            var eventsArticles =
                    entriesList
                    .Where(events => CheckDates(dateFrom, dateTo, events))
                    .OrderBy(o => o.EventDate)
                    .ThenBy(c => c.StartTime)
                    .ThenBy(t => t.Title)
                    .ToList();

            var eventCalender = new EventCalender();
            eventCalender.SetEvents(eventsArticles);

            return HttpResponse.Successful(eventCalender);
        }

        private bool CheckDates(DateTime? startDate, DateTime? endDate, Event events)
        {
            return startDate.HasValue && endDate.HasValue
                ? _dateComparer.EventDateIsBetweenStartAndEndDates(events.EventDate, startDate.Value, endDate.Value)
                : _dateComparer.EventDateIsBetweenTodayAndLater(events.EventDate);
        }
    }
}
