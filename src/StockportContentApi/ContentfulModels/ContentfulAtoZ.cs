using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulAtoZ : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string DisplayOnAZ { get; set; } = string.Empty;
        public List<string> AlternativeTitles { get; set; } = null;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
