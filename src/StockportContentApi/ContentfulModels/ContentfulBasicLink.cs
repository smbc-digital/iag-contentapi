using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulBasicLink : IContentfulModel
    {
        public string Text { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
