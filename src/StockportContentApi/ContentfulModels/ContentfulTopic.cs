using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulTopic
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public Asset BackgroundImage { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset"} };
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public List<Entry<ContentfulSubItem>> SubItems { get; set; } = new List<Entry<ContentfulSubItem>>();
        public List<Entry<ContentfulSubItem>> SecondaryItems { get; set; } = new List<Entry<ContentfulSubItem>>();
        public List<Entry<ContentfulSubItem>> TertiaryItems { get; set; } = new List<Entry<ContentfulSubItem>>();
        public List<Entry<ContentfulCrumb>> Breadcrumbs { get; set; } = new List<Entry<ContentfulCrumb>>();
        public List<Entry<ContentfulAlert>> Alerts { get; set; } = new List<Entry<ContentfulAlert>>();
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public bool EmailAlerts { get; set; } = false;
        public string EmailAlertsTopicId { get; set; } = string.Empty;
        public Entry<ContentfulEventBanner> EventBanner { get; set; } = new Entry<ContentfulEventBanner>
        {
            Fields = new ContentfulEventBanner(),
            SystemProperties = new SystemProperties { Type = "Entry" }
        };
    }
}
