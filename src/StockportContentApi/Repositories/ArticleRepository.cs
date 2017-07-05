using System;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Http;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Utils;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Client;
using Contentful.Core.Search;
using System.Linq;
using Contentful.Core.Models;
using System.Collections.Generic;

namespace StockportContentApi.Repositories
{
    public class ArticleRepository
    {        
        private readonly IContentfulFactory<ContentfulArticle, Article> _contentfulFactory;
        private readonly IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap> _contentfulFactoryArticle;
        private readonly DateComparer _dateComparer;      
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IVideoRepository _videoRepository;

        public ArticleRepository(ContentfulConfig config,
            IContentfulClientManager contentfulClientManager, 
            ITimeProvider timeProvider,
            IContentfulFactory<ContentfulArticle, Article> contentfulFactory,
            IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap> contentfulFactoryArticle,
            IVideoRepository videoRepository)
        {
            _contentfulFactory = contentfulFactory;
            _dateComparer = new DateComparer(timeProvider);
            _client = contentfulClientManager.GetClient(config);
            _videoRepository = videoRepository;
            _contentfulFactoryArticle = contentfulFactoryArticle;
        }

        public async Task<HttpResponse> Get()
        {
            var builder = new QueryBuilder<ContentfulArticleForSiteMap>().ContentTypeIs("article").Include(2).Limit(ContentfulQueryValues.LIMIT_MAX);
            var entries = await _client.GetEntriesAsync(builder);
            var contentfulArticles = entries as IEnumerable<ContentfulArticleForSiteMap> ?? entries.ToList();

            var articles = GetAllArticles(contentfulArticles.ToList())
                .Where(article => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(article.SunriseDate, article.SunsetDate));
            return entries == null || !contentfulArticles.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Articles found")
                : HttpResponse.Successful(articles);
        }

        public async Task<HttpResponse> GetArticle(string articleSlug)
        {
            var builder = new QueryBuilder<ContentfulArticle>()
                .ContentTypeIs("article")
                .FieldEquals("fields.slug", articleSlug)
                .Include(3);

            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();

            var articleItem = entry == null
                            ? null
                            : _contentfulFactory.ToModel(entry);

            if (articleItem != null)
            {
                foreach (var section in articleItem.Sections)
                {
                    if (section != null)
                        section.Body = _videoRepository.Process(section.Body);
                }

                articleItem.Body = _videoRepository.Process(articleItem.Body);

                if (!_dateComparer.DateNowIsWithinSunriseAndSunsetDates(articleItem.SunriseDate, articleItem.SunsetDate))
                {
                    articleItem = null;
                }         
            }

            return articleItem == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No article found for '{articleSlug}'")
                : HttpResponse.Successful(articleItem);          
        }

        private IEnumerable<ArticleSiteMap> GetAllArticles(List<ContentfulArticleForSiteMap> entries)
        {
            var entriesList = new List<ArticleSiteMap>();
            foreach (var entry in entries)
            {
                var articleItem = _contentfulFactoryArticle.ToModel(entry);
                entriesList.Add(articleItem);
            }

            return entriesList;
        }
    }
}
