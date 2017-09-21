using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class RedirectContentfulFactory : IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RedirectContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public BusinessIdToRedirects ToModel(ContentfulRedirect entry)
        {
            return new BusinessIdToRedirects(entry.Redirects, entry.LegacyUrls).StripData(_httpContextAccessor);
        }
    }
}