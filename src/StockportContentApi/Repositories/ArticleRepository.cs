using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Http;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Utils;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Client;
using Contentful.Core.Search;
using System.Linq;
using System.Collections.Generic;

namespace StockportContentApi.Repositories
{
    public class ArticleRepository : BaseRepository
    {        
        private readonly IContentfulFactory<ContentfulArticle, Article> _contentfulFactory;
        private readonly IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap> _contentfulFactoryArticle;
        private readonly DateComparer _dateComparer;
        private readonly ICache _cache;
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IVideoRepository _videoRepository;

        public ArticleRepository(ContentfulConfig config,
            IContentfulClientManager contentfulClientManager, 
            ITimeProvider timeProvider,
            IContentfulFactory<ContentfulArticle, Article> contentfulFactory,
            IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap> contentfulFactoryArticle,
            IVideoRepository videoRepository,
            ICache cache)
        {
            _contentfulFactory = contentfulFactory;
            _dateComparer = new DateComparer(timeProvider);
            _client = contentfulClientManager.GetClient(config);
            _videoRepository = videoRepository;
            _contentfulFactoryArticle = contentfulFactoryArticle;
            _cache = cache;
        }

        public async Task<HttpResponse> Get()
        {
            var builder = new QueryBuilder<ContentfulArticleForSiteMap>().ContentTypeIs("article").Include(2);
            var entries = await GetAllEntriesAsync(_client, builder);
            var contentfulArticles = entries as IEnumerable<ContentfulArticleForSiteMap> ?? entries.ToList();

            var articles = GetAllArticles(contentfulArticles.ToList())
                .Where(article => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(article.SunriseDate, article.SunsetDate));
            return entries == null || !contentfulArticles.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Articles found")
                : HttpResponse.Successful(articles);
        }

        public async Task<HttpResponse> GetArticle(string articleSlug)
        {
            var entry = await _cache.GetFromCacheOrDirectlyAsync("article-" + articleSlug, () => GetArticleEntry(articleSlug));
            
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

        private async Task<ContentfulArticle> GetArticleEntry(string articleSlug)
        {
            var builder = new QueryBuilder<ContentfulArticle>()
                .ContentTypeIs("article")
                .FieldEquals("fields.slug", articleSlug)
                .Include(3);

            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();
            return entry;
        }
    }
   
}
