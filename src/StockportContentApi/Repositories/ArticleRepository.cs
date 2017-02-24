using System;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Http;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class ArticleRepository
    {
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private readonly IFactory<Article> _articleFactory;
        private readonly IVideoRepository _videoRepository;
        private readonly ITimeProvider _timeProvider;
        private readonly DateComparer _dateComparer;

        public ArticleRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<Article> articleFactory, IVideoRepository videoRepository, ITimeProvider timeProvider)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _articleFactory = articleFactory;
            _videoRepository = videoRepository;
            _timeProvider = timeProvider;
            _dateComparer = new DateComparer(_timeProvider);
        }

        public async Task<HttpResponse> GetArticle(string articleSlug)
        {
            const int referenceLevelLimit = 2;
            var contentfulArticle = await _contentfulClient.Get(UrlFor("article", referenceLevelLimit, articleSlug));

            if (!contentfulArticle.HasItems())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No article found for '{articleSlug}'");

            var entry = contentfulArticle.GetFirstItem();
            var article = _articleFactory.Build(entry, contentfulArticle);

            foreach (var section in article.Sections)
            {
                section.Body = _videoRepository.Process(section.Body);
            }
            
           if (!_dateComparer.DateNowIsWithinSunriseAndSunsetDates(article.SunriseDate,article.SunsetDate)) article = new NullArticle();

            article.Body = _videoRepository.Process(article.Body);

            return article.GetType() == typeof(NullArticle)
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No article found for '{articleSlug}'")
                : HttpResponse.Successful(article);
        }

        private string UrlFor(string type, int referenceLevel, string slug = null)
        {
            return slug == null
                ? $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}"
                : $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}";
        }      
    }
}
