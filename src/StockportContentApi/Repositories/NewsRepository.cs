using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using Microsoft.Extensions.Configuration;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApi.Extensions;

namespace StockportContentApi.Repositories
{
    public class NewsRepository : BaseRepository
    {
        private readonly ITimeProvider _timeProvider;
        private const int ReferenceLevelLimit = 1;
        private readonly IContentfulFactory<ContentfulNews, News> _newsContentfulFactory;
        private readonly IContentfulFactory<ContentfulNewsRoom, Newsroom> _newsRoomContentfulFactory;
        private readonly DateComparer _dateComparer;
        private readonly ICache _cache;
        private readonly Contentful.Core.IContentfulClient _client;
        private IConfiguration _configuration;
        private readonly int _newsTimeout;

        public NewsRepository(ContentfulConfig config, ITimeProvider timeProvider, IContentfulClientManager contentfulClientManager,
            IContentfulFactory<ContentfulNews, News> newsContentfulFactory, IContentfulFactory<ContentfulNewsRoom, Newsroom> newsRoomContentfulFactory, ICache cache, IConfiguration configuration)
        {
            _timeProvider = timeProvider;
            _newsContentfulFactory = newsContentfulFactory;
            _newsRoomContentfulFactory = newsRoomContentfulFactory;
            _dateComparer = new DateComparer(timeProvider);
            _client = contentfulClientManager.GetClient(config);
            _cache = cache;
            _configuration = configuration;
            int.TryParse(_configuration["redisExpiryTimes:News"], out _newsTimeout);
        }

        private async Task<IList<ContentfulNews>> GetAllNews()
        {
            var builder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1);
            var entries = await GetAllEntriesAsync(_client, builder);
            return !entries.Any() ? null : entries.ToList();
        }

        private async Task<ContentfulNewsRoom> GetNewsRoom()
        {
            var builder = new QueryBuilder<ContentfulNewsRoom>().ContentTypeIs("newsroom").Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            return entries.FirstOrDefault();
        }

        private async Task<List<string>> GetNewsCategories()
        {
            var eventType = await _client.GetContentTypeAsync("news");
            var validation = eventType.Fields.First(f => f.Name == "Categories").Items.Validations[0] as Contentful.Core.Models.Management.InValuesValidator;
            return validation?.RequiredValues;
        }

        public async Task<HttpResponse> GetNews(string slug)
        {
            var entries = await _cache.GetFromCacheOrDirectlyAsync("news-all", GetAllNews, _newsTimeout);

            var entry = entries.Where(e => e.Slug == slug).FirstOrDefault();

            if (entry != null && !_dateComparer.DateNowIsAfterSunriseDate(entry.SunriseDate)) entry = null;

            return entry == null 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No news found for '{slug}'") 
                : HttpResponse.Successful(_newsContentfulFactory.ToModel(entry));
        }

        public async Task<HttpResponse> Get(string tag, string category, DateTime? startDate, DateTime? endDate)
        {
            var newsroom = new Newsroom(new List<Alert>(), false, string.Empty);

            var newsRoomEntry = await _cache.GetFromCacheOrDirectlyAsync("newsroom", GetNewsRoom, _newsTimeout);

            List<string> categories;

            if (newsRoomEntry != null)
            {
                newsroom = _newsRoomContentfulFactory.ToModel(newsRoomEntry);
            }
           
            var newsEntries = await _cache.GetFromCacheOrDirectlyAsync("news-all", GetAllNews, _newsTimeout);
            var filteredEntries = newsEntries.Where(n => tag == null || n.Tags.Any(t => t == tag));

            if (!filteredEntries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");

            List<DateTime> dates;

            var newsArticles = filteredEntries
                .Select(item => _newsContentfulFactory.ToModel(item))
                .GetNewsDates(out dates, _timeProvider)
                .Where(news => CheckDates(startDate, endDate, news))
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
            var newsEntries = await _cache.GetFromCacheOrDirectlyAsync("news-all", GetAllNews, _newsTimeout);

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
            var result = await _cache.GetFromCacheOrDirectlyAsync("news-categories", GetNewsCategories, _newsTimeout);
            return result;
        }
    }
}
