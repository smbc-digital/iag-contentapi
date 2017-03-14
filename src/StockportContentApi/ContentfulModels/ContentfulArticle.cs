using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulArticle
    {
        public string Body { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public Asset BackgroundImage { get; set; } = new Asset
        {
            File = new File { Url = string.Empty },
            SystemProperties = new SystemProperties { Type = "Asset" }
        };
        public Asset Image { get; set; } = new Asset
        {
            File = new File { Url = string.Empty },
            SystemProperties = new SystemProperties { Type = "Asset" }
        };
        public List<Entry<ContentfulSection>> Sections { get; set; } = new List<Entry<ContentfulSection>>();
        public List<Entry<ContentfulCrumb>> Breadcrumbs { get; set; } = new List<Entry<ContentfulCrumb>>();
        public List<Entry<Alert>> Alerts { get; set; } = new List<Entry<Alert>>();
        public List<Entry<ContentfulProfile>> Profiles { get; set; } = new List<Entry<ContentfulProfile>>();
        public Entry<ContentfulTopic> ParentTopic { get; set; } = new Entry<ContentfulTopic>
        {
            Fields = new ContentfulTopic(),
            SystemProperties = new SystemProperties { Type = "Entry" }
        };
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public List<Entry<Alert>> AlertsInline { get; set; } = new List<Entry<Alert>>();
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public bool LiveChatVisible { get; set; } = false;
        public Entry<LiveChat> LiveChatText { get; set; } = new Entry<LiveChat>
        {
            Fields = new NullLiveChat(),
            SystemProperties = new SystemProperties { Type = "Entry" }
        };
    }
}