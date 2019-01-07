using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulDidYouKnow : IContentfulModel
    {
        public string Name { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string Link { get; set; } = string.Empty;

        public SystemProperties Sys { get; set; }
    }
}
