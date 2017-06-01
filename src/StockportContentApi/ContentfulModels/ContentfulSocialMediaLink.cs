using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using System;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulSocialMediaLink
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
