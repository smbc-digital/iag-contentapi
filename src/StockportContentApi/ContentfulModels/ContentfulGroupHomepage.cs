using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulGroupHomepage : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public Asset BackgroundImage { get; set; } = new Asset
        {
            File = new File { Url = string.Empty },
            SystemProperties = new SystemProperties { Type = "Asset" }
        };
        public string FeaturedGroupsHeading { get; set; } = string.Empty;
        public List<ContentfulGroup> FeaturedGroups { get; set; } = new List<ContentfulGroup>();
        public ContentfulGroupCategory FeaturedGroupsCategory { get; set; } = new ContentfulGroupCategory();
        public ContentfulGroupSubCategory FeaturedGroupsSubCategory { get; set; } = new ContentfulGroupSubCategory();
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
