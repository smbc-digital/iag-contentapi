using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulSpotlightBanner : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
