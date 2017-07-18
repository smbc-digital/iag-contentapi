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
        private readonly ITimeProvider _timeProvider;
        private const int ReferenceLevelLimit = 1;
        private readonly IContentfulFactory<ContentfulNews, News> _newsContentfulFactory;
        private readonly IContentfulFactory<ContentfulNewsRoom, Newsroom> _newsRoomContentfulFactory;
        private readonly DateComparer _dateComparer;
        private readonly Contentful.Core.IContentfulClient _client;
        
        public NewsRepository(ContentfulConfig config, ITimeProvider timeProvider, IContentfulClientManager contentfulClientManager,
            IContentfulFactory<ContentfulNews, News> newsContentfulFactory, IContentfulFactory<ContentfulNewsRoom, Newsroom> newsRoomContentfulFactory)
        {
            _timeProvider = timeProvider;
            _newsContentfulFactory = newsContentfulFactory;
            _newsRoomContentfulFactory = newsRoomContentfulFactory;
            _dateComparer = new DateComparer(timeProvider);
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
                : HttpResponse.Successful(_newsContentfulFactory.ToModel(entry));
        }

        public async Task<HttpResponse> Get(string tag, string category, DateTime? startDate, DateTime? endDate)
        {
            var newsroom = new Newsroom(new List<Alert>(), false, string.Empty);
            var newsRoomBuilder = new QueryBuilder<ContentfulNewsRoom>().ContentTypeIs("newsroom").Include(1);
            var newsRoomEntries = await _client.GetEntriesAsync(newsRoomBuilder);

            List<string> categories;

            if (newsRoomEntries.Any())
            {
                newsroom = _newsRoomContentfulFactory.ToModel(newsRoomEntries.FirstOrDefault());
            }
           
            var newsBuilder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(ReferenceLevelLimit).Limit(ContentfulQueryValues.LIMIT_MAX);
            var newsEntries = await _client.GetEntriesAsync(newsBuilder);

            if (!newsEntries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");

            List<DateTime> dates;

            var newsArticles = newsEntries
                .Select(item => _newsContentfulFactory.ToModel(item))
                .GetNewsDates(out dates, _timeProvider)
                .Where(news => CheckDates(startDate, endDate, news))
                .GetTheCategories(out categories)
                .Where(news => string.IsNullOrWhiteSpace(category) || news.Categories.Contains(category))
                .Where(news => string.IsNullOrWhiteSpace(tag) || FilterNewsByTag(tag, news))
                .OrderByDescending(o => o.SunriseDate)
                .ToList();

        categories = await GetCategories();

            newsroom.SetNews(newsArticles);
            newsroom.SetCategories(categories);
            newsroom.SetDates(dates.Distinct().ToList());

            return HttpResponse.Successful(newsroom);
        }

        private bool FilterNewsByTag(string tag, News news)
        {
            if (tag.StartsWith("#"))
            {
                tag = tag.Remove(0, 1);
            }

            var result = news.Tags.Contains(tag);
            return result;
        }

        private bool CheckDates(DateTime? startDate, DateTime? endDate, News news)
        {
            return startDate.HasValue && endDate.HasValue 
                ? _dateComparer.SunriseDateIsBetweenStartAndEndDates(news.SunriseDate, startDate.Value, endDate.Value) 
                : _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate);
        }

        public async Task<HttpResponse> GetNewsByLimit(int limit)
        {
            var newsBuilder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(ReferenceLevelLimit).Limit(ContentfulQueryValues.LIMIT_MAX);
            var newsEntries = await _client.GetEntriesAsync(newsBuilder);

            if (!newsEntries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");
            var newsArticles = newsEntries
                .Select(item => _newsContentfulFactory.ToModel(item))
                .Where(news => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate))
                .OrderByDescending(o => o.SunriseDate)
                .Take(limit)
                .ToList();

            return !newsArticles.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No news found")
                : HttpResponse.Successful(newsArticles);
        }        

        private static string GetSearchTypeForTag(ref string tag)
        {            
            if (string.IsNullOrEmpty(tag) || !tag.StartsWith("#")) return "in";
            tag = tag.Remove(0, 1);

            return "match";
        }

        public async Task<List<string>> GetCategories()
        {
            var eventType = await _client.GetContentTypeAsync("news");
            var validation = eventType.Fields.First(f => f.Name == "Categories").Items.Validations[0] as Contentful.Core.Models.Management.InValuesValidator;
            return validation?.RequiredValues;
        }
    }
}
