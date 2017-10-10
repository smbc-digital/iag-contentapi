using System;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulAlert : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string SubHeading { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public SystemProperties Sys { get; set; } = new SystemProperties();
        public string Slug { get; set; } = string.Empty;

    }
}