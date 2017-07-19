using Contentful.Core.Models;
using StockportContentApi.Model;
using System.Collections.Generic;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulHomepage : IContentfulModel
    {
        public IEnumerable<string> PopularSearchTerms { get; set; } = new List<string>();
        public string FeaturedTasksHeading { get; set; } = string.Empty;
        public string FeaturedTasksSummary { get; set; } = string.Empty;
        public IEnumerable<ContentfulReference> FeaturedTasks { get; set; } = new List<ContentfulReference>();
        public IEnumerable<ContentfulReference> FeaturedTopics { get; set; } = new List<ContentfulReference>();
        public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public IEnumerable<ContentfulCarouselContent> CarouselContents { get; set; } = new List<ContentfulCarouselContent>();
        public Asset BackgroundImage { get; set; } = new Asset();
        public string FreeText { get; set; } = string.Empty;
        public ContentfulGroup FeaturedGroup { get; set; } = null;
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
