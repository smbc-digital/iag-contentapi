using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class RedirectContentfulFactory : IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>
    {
        public BusinessIdToRedirects ToModel(ContentfulRedirect entry)
        {
            return new BusinessIdToRedirects(entry.Redirects, entry.LegacyUrls);
        }
    }
}