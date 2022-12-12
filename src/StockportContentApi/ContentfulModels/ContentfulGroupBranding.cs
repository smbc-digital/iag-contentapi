using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulGroupBranding : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public Asset File { get; set; } = new Asset();

        public string Url { get; set; } = string.Empty;

        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}