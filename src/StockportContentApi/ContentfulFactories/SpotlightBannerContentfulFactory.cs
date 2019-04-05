using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class SpotlightBannerContentfulFactory : IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SpotlightBannerContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public SpotlightBanner ToModel(ContentfulSpotlightBanner entry)
        {
            return new SpotlightBanner(entry.Title, entry.Teaser, entry.Link).StripData(_httpContextAccessor);
        }
    }
}
