using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class EventBannerContentfulFactory : IContentfulFactory<ContentfulEventBanner, EventBanner>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventBannerContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public EventBanner ToModel(ContentfulEventBanner entry)
        {
            return new EventBanner(entry.Title, entry.Teaser, entry.Icon, entry.Link).StripData(_httpContextAccessor);
        }
    }
}
