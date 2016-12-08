using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApi.Extensions;

namespace StockportContentApi.Repositories
{
    public class EventRepository
    {
        private readonly IFactory<Event> _eventFactory;       
        private readonly ITimeProvider _timeProvider;
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private readonly DateComparer _dateComparer;
        private const int ReferenceLevelLimit = 1;

        public EventRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<Event> eventFactory,ITimeProvider timeProvider)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _eventFactory = eventFactory;
            _timeProvider = timeProvider;
            _dateComparer = new DateComparer(timeProvider);
        }

        public async Task<HttpResponse> Get(string tag, string category, DateTime? startDate, DateTime? endDate)
        {
            var eventCalender = new Newsroom(new List<Alert>(), false, string.Empty);
            //var newsroomContentfulResponse = await _contentfulClient.Get(UrlForNewsroom("newsroom", ReferenceLevelLimit));

            //if (newsroomContentfulResponse.HasItems())
            //{
            //    newsroom = _newsroomFactory.Build(newsroomContentfulResponse.GetFirstItem(), newsroomContentfulResponse);
            //}

            //var newsContentfulResponse = await _contentfulClient.Get(UrlForNewsWithFilters("news", ReferenceLevelLimit, tag));

            //if (!newsContentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");

            //List<string> categories;
            //List<DateTime> dates;
            //var newsArticles = newsContentfulResponse.GetAllItems()
            //    .Select(item => _newsFactory.Build(item, newsContentfulResponse))
            //    .Cast<News>()
            //    .GetNewsDates(out dates, _timeProvider)
            //    .Where(news => CheckDates(startDate, endDate, news))
            //    .GetTheCategories(out categories)
            //    .Where(news => string.IsNullOrWhiteSpace(category) || news.Categories.Contains(category))
            //    .OrderByDescending(o => o.SunriseDate)
            //    .ToList();

            //newsroom.SetNews(newsArticles);
            //newsroom.SetCategories(categories.Distinct().ToList());
            //newsroom.SetDates(dates.Distinct().ToList());

            return HttpResponse.Successful(newsroom);
        }


        public async Task<HttpResponse> GetEvent(string slug)
        {
            var contentfulResponse = await _contentfulClient.Get(UrlForSlug("event", ReferenceLevelLimit, slug));

            if (!contentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No event found for '{slug}'");

            Event eventItem = _eventFactory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

       

            return eventItem.GetType() == typeof(NullEvent)
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No event found for '{slug}'")
                : HttpResponse.Successful(eventItem);
        }

        private string UrlForSlug(string type, int referenceLevel, string slug)
        {
            return $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}";
        }
    }
}
