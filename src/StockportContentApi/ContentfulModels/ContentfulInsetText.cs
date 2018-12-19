using System;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulInsetText : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string SubHeading { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Colour { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
        public string Slug { get; set; } = string.Empty;
    }
}