using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories.ArticleFactories
{
    public class ArticleSiteMapContentfulFactory : IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticleSiteMapContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ArticleSiteMap ToModel(ContentfulArticleForSiteMap entry)
        {
            return new ArticleSiteMap(entry.Slug, entry.SunriseDate, entry.SunsetDate).StripData(_httpContextAccessor);
        }
    }
}