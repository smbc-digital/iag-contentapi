using System;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulAdvertisment : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public bool IsAdvertisment { get; set; } = true;
        public string NavigartionUrl { get; set; } = string.Empty;
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}