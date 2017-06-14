using System;
using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulSection : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<ContentfulProfile> Profiles { get; set; } = new List<ContentfulProfile>();
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public List<ContentfulAlert> AlertsInline { get; set; } = new List<ContentfulAlert>();
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
