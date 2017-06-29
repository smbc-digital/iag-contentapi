using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ArticleSiteMapContentfulFactory : IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>
    {
        public ArticleSiteMapContentfulFactory()
        {
        }

        public ArticleSiteMap ToModel(ContentfulArticleForSiteMap entry)
        {
            return new ArticleSiteMap(entry.Slug);
        }
    }
}