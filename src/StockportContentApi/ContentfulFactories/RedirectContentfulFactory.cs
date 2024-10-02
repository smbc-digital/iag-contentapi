namespace StockportContentApi.ContentfulFactories;

public class RedirectContentfulFactory : IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>
{
    public BusinessIdToRedirects ToModel(ContentfulRedirect entry) =>
        new BusinessIdToRedirects(entry.Redirects, entry.LegacyUrls);
}