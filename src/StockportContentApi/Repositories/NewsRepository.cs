using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApi.Extensions;

namespace StockportContentApi.Repositories
{
    public class NewsRepository
    {
        private readonly IFactory<News> _newsFactory;
        private readonly IFactory<Newsroom> _newsroomFactory;
        private readonly ITimeProvider _timeProvider;
        private readonly INewsCategoriesFactory _newsCategoryFactory;
        private readonly string _contentfulApiUrl;
        private readonly string _contentfulContentTypesUrl;
        private readonly ContentfulClient _contentfulClient;
        private const int ReferenceLevelLimit = 1;
        private readonly IContentfulFactory<ContentfulNews, News> _contentfulFactory;
        private readonly DateComparer _dateComparer;
        private readonly Contentful.Core.IContentfulClient _client;

        public NewsRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<News> newsFactory, 
            IFactory<Newsroom> newsroomFactory, INewsCategoriesFactory newsCategoriesFactory,
            ITimeProvider timeProvider, IContentfulClientManager contentfulClientManager,
            IContentfulFactory<ContentfulNews, News> contentfulFactory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _contentfulContentTypesUrl = config.ContentfulContentTypesUrl.ToString();
            _newsFactory = newsFactory;
            _newsroomFactory = newsroomFactory;
            _timeProvider = timeProvider;
            _contentfulFactory = contentfulFactory;
            _dateComparer = new DateComparer(timeProvider);
            _newsCategoryFactory = newsCategoriesFactory;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetNews(string slug)
        {
            var builder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            if (entry != null && !_dateComparer.DateNowIsAfterSunriseDate(entry.SunriseDate)) entry = null;

            return entry == null 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No news found for '{slug}'") 
                : HttpResponse.Successful(_contentfulFactory.ToModel(entry));
        }

        public async Task<HttpResponse> Get(string tag, string category, DateTime? startDate, DateTime? endDate)
        {
            var newsroom = new Newsroom(new List<Alert>(), false, string.Empty);
            var newsroomContentfulResponse = await _contentfulClient.Get(UrlFor("newsroom", ReferenceLevelLimit));
            List<string> categories;

            if (newsroomContentfulResponse.HasItems())
            {
                newsroom = _newsroomFactory.Build(newsroomContentfulResponse.GetFirstItem(), newsroomContentfulResponse);
            }

            var newsContentfulResponse = await _contentfulClient.Get(UrlFor("news", ReferenceLevelLimit, tag: tag, limit: ContentfulQueryValues.LIMIT_MAX));
           
            if (!newsContentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");           

            List<DateTime> dates;            

            var newsArticles = newsContentfulResponse.GetAllItems()
                .Select(item => _newsFactory.Build(item, newsContentfulResponse))
                .Cast<News>()
                .GetNewsDates(out dates, _timeProvider)
                .Where(news => CheckDates(startDate, endDate, news))
                .GetTheCategories(out categories)
                .Where(news => string.IsNullOrWhiteSpace(category) || news.Categories.Contains(category))
                .OrderByDescending(o => o.SunriseDate)
                .ToList();

            categories = await GetCategories();
         
            newsroom.SetNews(newsArticles);
            newsroom.SetCategories(categories);
            newsroom.SetDates(dates.Distinct().ToList());

            return HttpResponse.Successful(newsroom);
        }

        private bool CheckDates(DateTime? startDate, DateTime? endDate, News news)
        {
            return startDate.HasValue && endDate.HasValue 
                ? _dateComparer.SunriseDateIsBetweenStartAndEndDates(news.SunriseDate, startDate.Value, endDate.Value) 
                : _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate);
        }

        public async Task<HttpResponse> GetNewsByLimit(int limit)
        {
            var contentfulResponse = await _contentfulClient.Get(UrlFor("news", ReferenceLevelLimit, limit: ContentfulQueryValues.LIMIT_MAX));

            if (!contentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");
            var newsArticles = contentfulResponse.GetAllItems()
                .Select(item => _newsFactory.Build(item, contentfulResponse))
                .Cast<News>()
                .Where(news => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate))
                .OrderByDescending(o => o.SunriseDate)
                .Take(limit)
                .ToList();

            return !newsArticles.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No news found")
                : HttpResponse.Successful(newsArticles);
        }

        private string UrlFor(string type, int referenceLevel, string slug = null, int limit = -1, string tag = null)
        {
            var baseUrl = $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}";

            if (!string.IsNullOrWhiteSpace(slug)) baseUrl = $"{baseUrl}&fields.slug={slug}";

            if (!string.IsNullOrWhiteSpace(tag))
                baseUrl = string.Concat(baseUrl, $"&fields.tags[{GetSearchTypeForTag(ref tag)}]={tag}");

            if (limit >= 0) baseUrl = $"{baseUrl}&limit={limit}";

            return baseUrl;
        }

        private static string GetSearchTypeForTag(ref string tag)
        {            
            if (string.IsNullOrEmpty(tag) || !tag.StartsWith("#")) return "in";
            tag = tag.Remove(0, 1);

            return "match";
        }

        public async Task <List<string>>GetCategories()
        {           
            var contentfulResponse = await _contentfulClient.Get(_contentfulContentTypesUrl);
            var contentfulData = contentfulResponse.Items;
            var newsCategories = _newsCategoryFactory.Build(contentfulData);
            return newsCategories;
        }
    }
}
