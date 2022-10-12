using Contentful.Core.Models;
using StockportContentApi.Model;
using System.Collections.Generic;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulEventHomepage : IContentfulModel
    {
        public string Tag1 { get; set; } = string.Empty;
        public string Tag2 { get; set; } = string.Empty;
        public string Tag3 { get; set; } = string.Empty;
        public string Tag4 { get; set; } = string.Empty;
        public string Tag5 { get; set; } = string.Empty;
        public string Tag6 { get; set; } = string.Empty;
        public string Tag7 { get; set; } = string.Empty;
        public string Tag8 { get; set; } = string.Empty;
        public string Tag9 { get; set; } = string.Empty;
        public string Tag10 { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();

        public string MetaDescription { get; set; } = string.Empty;

        public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();

    }
}
