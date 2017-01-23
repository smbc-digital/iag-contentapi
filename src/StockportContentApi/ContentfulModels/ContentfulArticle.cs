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
        public Asset BackgroundImage { get; set; } = new Asset { File = new File { Url = string.Empty } };
        public List<ContentfulSection> Sections { get; set; } = new List<ContentfulSection>();
        public List<Entry<ContentfulCrumb>> Breadcrumbs { get; set; } = new List<Entry<ContentfulCrumb>>();
        public List<Alert> Alerts { get; set; } = new List<Alert>();
        public List<ContentfulProfile> Profiles { get; set; } = new List<ContentfulProfile>();
        public ContentfulTopic ParentTopic { get; set; } = new ContentfulTopic();
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public bool LiveChatVisible { get; set; } = false;
        public LiveChat LiveChat { get; set; } = new NullLiveChat();
    }
}