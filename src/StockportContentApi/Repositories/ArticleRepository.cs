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

namespace StockportContentApi.Repositories
{
    public class ArticleRepository
    {        
        private readonly IContentfulFactory<ContentfulArticle, Article> _contentfulFactory;
        private readonly DateComparer _dateComparer;      
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IVideoRepository _videoRepository;

        public ArticleRepository(ContentfulConfig config,
            IContentfulClientManager contentfulClientManager, 
            ITimeProvider timeProvider,
            IContentfulFactory<ContentfulArticle, Article> contentfulFactory, 
            IVideoRepository videoRepository)
        {
            _contentfulFactory = contentfulFactory;
            _dateComparer = new DateComparer(timeProvider);
            _client = contentfulClientManager.GetClient(config);
            _videoRepository = videoRepository;
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
    }
}
