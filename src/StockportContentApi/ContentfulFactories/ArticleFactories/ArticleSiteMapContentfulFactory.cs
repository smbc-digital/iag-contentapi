namespace StockportContentApi.ContentfulFactories.ArticleFactories;

public class ArticleSiteMapContentfulFactory : IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>
{
    public ArticleSiteMap ToModel(ContentfulArticleForSiteMap entry)
    {
        return new ArticleSiteMap(entry.Slug, entry.SunriseDate, entry.SunsetDate);
    }
}