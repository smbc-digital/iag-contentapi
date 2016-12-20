using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        private readonly IFactory<EventCalender> _eventCalanderFactory;
        private readonly ITimeProvider _timeProvider;
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private readonly DateComparer _dateComparer;
        private const int ReferenceLevelLimit = 1;

        public EventRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<Event> eventFactory, IFactory<EventCalender> evenCalanderFactory, ITimeProvider timeProvider)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _eventFactory = eventFactory;
            _eventCalanderFactory = evenCalanderFactory;
            _timeProvider = timeProvider;
            _dateComparer = new DateComparer(timeProvider);
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

            return !eventsArticles.Any() ? HttpResponse.Failure(HttpStatusCode.NotFound, "No events found") : HttpResponse.Successful(eventCalender);         
        }

        public async Task<HttpResponse> GetEvent(string slug)
        {
            var contentfulResponse = await _contentfulClient.Get(UrlForSlug("events", ReferenceLevelLimit, slug));

            if (!contentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No event found for '{slug}'");

            Event eventItem = _eventFactory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            if (!_dateComparer.DateNowIsWithinSunriseAndSunsetDates(eventItem.SunriseDate, eventItem.SunsetDate)) eventItem = new NullEvent();

            return eventItem.GetType() == typeof(NullEvent)
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No event found for '{slug}'")
                : HttpResponse.Successful(eventItem);
        }

        private bool CheckDates(Event eventItem)
        {
            return _dateComparer.DateNowIsWithinSunriseAndSunsetDates(eventItem.SunriseDate, eventItem.SunsetDate);
        }

        private string UrlForEvents(string type, int referenceLevel)
        {
            var baseUrl = $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}";                 
            return baseUrl;
        }

        private string UrlForSlug(string type, int referenceLevel, string slug)
        {
            return $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}";
        }

        private string UrlForEventCalander(string type, int referenceLevel)
        {
            return $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}";
        }

        private bool CheckDates(DateTime? startDate, DateTime? endDate, Event events)
        {
            return startDate.HasValue && endDate.HasValue
                ? _dateComparer.EventDateIsBetweenStartAndEndDates(events.EventDate, startDate.Value, endDate.Value)
                : _dateComparer.EventDateIsBetweenTodayAndLater(events.EventDate);
        }
    }
}
