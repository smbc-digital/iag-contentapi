using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulCallToActionBanner
    {
        public string Title { get; set; } = string.Empty;
        public Asset Image { get; set; } = null;
        public string Link { get; set; } = string.Empty;
        public string ButtonText { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }
}