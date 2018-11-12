using System;
using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulNews : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public string Body { get; set; } = string.Empty;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public List<string> Tags { get; set; } = new List<string>();
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public List<string> Categories { get; set; } = new List<string>();
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}