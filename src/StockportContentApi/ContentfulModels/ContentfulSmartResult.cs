using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulSmartResult : IContentfulModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Subheading { get; set; }
        public string Icon { get; set; }
        public string Body { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public SystemProperties Sys { get; set; }
    }
}
