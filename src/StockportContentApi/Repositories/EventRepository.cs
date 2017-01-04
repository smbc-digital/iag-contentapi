using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class EventRepository
    {
        private readonly IFactory<Event> _eventFactory;
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private readonly DateComparer _dateComparer;
        private readonly Contentful.Core.IContentfulClient _client;
        private const int ReferenceLevelLimit = 1;

        public EventRepository(ContentfulConfig config, IHttpClient httpClient, IContentfulClientManager contentfulClientManager, IFactory<Event> eventFactory, ITimeProvider timeProvider)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _eventFactory = eventFactory;
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
            var eventCalender = new EventCalender();

            var eventsContentfulResponse = await _contentfulClient.Get(UrlForEvents("events", ReferenceLevelLimit));

            if (!eventsContentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No events found");

            var eventsArticles =
                eventsContentfulResponse.GetAllItems()
                    .Select(item => _eventFactory.Build(item, eventsContentfulResponse))
                    .Cast<Event>()
                    .Where(events => CheckDates(dateFrom, dateTo, events))
                    .OrderBy(o => o.EventDate)
                    .ThenBy(c => c.StartTime)
                    .ThenBy(t => t.Title)
                    .ToList();

            eventCalender.SetEvents(eventsArticles);
  
            return HttpResponse.Successful(eventCalender);
        }

        private string UrlForEvents(string type, int referenceLevel)
        {
            var baseUrl = $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}";                 
            return baseUrl;
        }

        private bool CheckDates(DateTime? startDate, DateTime? endDate, Event events)
        {
            return startDate.HasValue && endDate.HasValue
                ? _dateComparer.EventDateIsBetweenStartAndEndDates(events.EventDate, startDate.Value, endDate.Value)
                : _dateComparer.EventDateIsBetweenTodayAndLater(events.EventDate);
        }
    }
}
