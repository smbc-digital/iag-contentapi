using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulLiveChat : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}