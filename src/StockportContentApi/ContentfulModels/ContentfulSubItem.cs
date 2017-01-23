using System;
using Contentful.Core.Configuration;
using Newtonsoft.Json;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulSubItem
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
    }
}