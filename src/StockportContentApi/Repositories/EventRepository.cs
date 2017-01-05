using System;
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

        public async Task<HttpResponse> GetEvent(string slug)
        {
            var builder = new QueryBuilder().ContentTypeIs("events").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync<Event>(builder);

            var entry = entries.FirstOrDefault();
            if (entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No event found for '{slug}'");

            return !_dateComparer.DateNowIsWithinSunriseAndSunsetDates(entry.SunriseDate, entry.SunsetDate) 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No event found for '{slug}'") 
                : HttpResponse.Successful(entry);
        }

        public async Task<HttpResponse> Get(DateTime? dateFrom, DateTime? dateTo)
        {
            var builder = new QueryBuilder().ContentTypeIs("events").Include(1);
            var entries = await _client.GetEntriesAsync<Event>(builder);

            if (entries == null || !entries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No events found");

            var eventsArticles =
                    entries
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
