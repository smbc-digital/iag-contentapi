using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulGroupBranding : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public MediaAsset File { get; set; } = new MediaAsset();

        public string Url { get; set; } = string.Empty;

        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}