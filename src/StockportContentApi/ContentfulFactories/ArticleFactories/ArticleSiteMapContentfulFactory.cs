namespace StockportContentApi.ContentfulFactories.ArticleFactories;

public class ArticleSiteMapContentfulFactory : IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>
{
    public ArticleSiteMap ToModel(ContentfulArticleForSiteMap entry) =>
        new(entry.Slug, entry.SunriseDate, entry.SunsetDate);
}