using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulSection
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<ContentfulProfile> Profiles { get; set; } = new List<ContentfulProfile>();
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
    }
}
