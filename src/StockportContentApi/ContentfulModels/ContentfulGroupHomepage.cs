using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulGroupHomepage : ContentfulReference
    {
        public string FeaturedGroupsHeading { get; set; } = string.Empty;
        public List<ContentfulGroup> FeaturedGroups { get; set; } = new List<ContentfulGroup>();
        public ContentfulGroupCategory FeaturedGroupsCategory { get; set; } = new ContentfulGroupCategory();
        public ContentfulGroupSubCategory FeaturedGroupsSubCategory { get; set; } = new ContentfulGroupSubCategory();
        public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public string BodyHeading { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string SecondaryBodyHeading { get; set; } = string.Empty;
        public string SecondaryBody { get; set; } = string.Empty;
        public ContentfulEventBanner EventBanner { get; set; } = new ContentfulEventBanner
        {
            Sys = new SystemProperties { Type = "Entry" }
        };
    }
}
