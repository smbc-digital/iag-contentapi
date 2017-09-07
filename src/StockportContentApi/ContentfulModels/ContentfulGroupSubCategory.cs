using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulGroupSubCategory : IContentfulModel
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
