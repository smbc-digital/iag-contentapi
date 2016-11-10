﻿using System;
using System.Collections;
using System.Collections.Generic;
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
    public class NewsRepository
    {
        private readonly IFactory<News> _newsFactory;
        private readonly IFactory<Newsroom> _newsroomFactory;
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private const int ReferenceLevelLimit = 1;
        private readonly IVideoRepository _videoRepository;
        private readonly SunriseSunsetDates _sunriseSunsetDates;

        public NewsRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<News> newsFactory, IFactory<Newsroom> newsroomFactory, ITimeProvider timeProvider, IVideoRepository videoRepository)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _newsFactory = newsFactory;
            _newsroomFactory = newsroomFactory;
            _videoRepository = videoRepository;
            _sunriseSunsetDates = new SunriseSunsetDates(timeProvider);
        }

        public async Task<HttpResponse> Get(string tag, string category)
        {
            var newsroom = new Newsroom(new List<Alert>(), false, string.Empty);
            var newsroomContentfulResponse = await _contentfulClient.Get(UrlForNewsroom("newsroom", ReferenceLevelLimit));

            if (newsroomContentfulResponse.HasItems())
            {
                newsroom = _newsroomFactory.Build(newsroomContentfulResponse.GetFirstItem(), newsroomContentfulResponse);
            }

            var newsContentfulResponse = await _contentfulClient.Get(UrlForNewsWithFilters("news", ReferenceLevelLimit, tag));

            if (!newsContentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");

            List<string> categories;

            var newsArticles = newsContentfulResponse.GetAllItems()
                .Select(item => _newsFactory.Build(item, newsContentfulResponse))
                .Cast<News>()
                .Where(news => _sunriseSunsetDates.CheckIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate))
                .GetTheCategories(out categories)
                .Where(news => string.IsNullOrWhiteSpace(category) || news.Categories.Contains(category))
                .OrderByDescending(o => o.SunriseDate)
                .ToList();
         
            newsroom.SetNews(newsArticles);
            newsroom.SetCategories(categories.Distinct().ToList());

            return HttpResponse.Successful(newsroom);
        }

        public async Task<HttpResponse> GetNews(string slug)
        {
            var contentfulResponse = await _contentfulClient.Get(UrlForSlug("news", ReferenceLevelLimit, slug));

            if (!contentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No news found for '{slug}'");

            News news = _newsFactory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            news.Body = _videoRepository.Process(news.Body);

            if (!_sunriseSunsetDates.CheckIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate)) news = new NullNews();

            return news.GetType() == typeof(NullNews)
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No news found for '{slug}'")
                : HttpResponse.Successful(news);
        }

        public async Task<HttpResponse> GetNewsByLimit(int limit)
        {
            var url = UrlForNews("news", ReferenceLevelLimit);
            var contentfulResponse = await _contentfulClient.Get(url);

            if (!contentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");
            var newsArticles = contentfulResponse.GetAllItems()
                .Select(item => _newsFactory.Build(item, contentfulResponse))
                .Cast<News>()
                .Where(news => _sunriseSunsetDates.CheckIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate))
                .OrderByDescending(o => o.SunriseDate)
                .Take(limit)
                .ToList();

            return !newsArticles.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No news found")
                : HttpResponse.Successful(newsArticles);
        }

        private string UrlForNews(string type, int referenceLevel)
        {
            return $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}";
        }

        private string UrlForNewsroom(string type, int referenceLevel)
        {
            return $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}";
        }

        private string UrlForNewsWithFilters(string type, int referenceLevel, string tag)
        {
            var baseUrl = $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}";

            var querys = "";
            querys += !string.IsNullOrWhiteSpace(tag) ? CreateQueryParameter("fields.tags[in]", tag) : string.Empty;

            var url = string.Concat(baseUrl, querys);
            return url;
        }

        private static string CreateQueryParameter(string queryName, string queryValue)
        {
            return string.Concat("&", queryName, "=", queryValue);
        }

        private string UrlForSlug(string type, int referenceLevel, string slug)
        {
            return $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}";
        }
    }

    public static class NewsExtensions
    {
        public static IEnumerable<News> GetTheCategories(this IEnumerable<News> news, out List<string> categories)
        {
            var categoriesList = new List<string>();

            news.ToList().ForEach(n => categoriesList.AddRange(n.Categories));

            categories = categoriesList;
            return news;
        }
    }
}
