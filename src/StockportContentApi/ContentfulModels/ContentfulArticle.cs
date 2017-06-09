using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    
    public class ContentfulArticle
    {
        public string Body { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
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
        public List<ContentfulSection> Sections { get; set; } = new List<ContentfulSection>();
        public List<ContentfulCrumb> Breadcrumbs { get; set; } = new List<ContentfulCrumb>();
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public List<ContentfulProfile> Profiles { get; set; } = new List<ContentfulProfile>();
        public ContentfulTopic ParentTopic { get; set; } = new ContentfulTopic
        {
            Sys = new SystemProperties { Type = "Entry" }
        };
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public List<ContentfulAlert> AlertsInline { get; set; } = new List<ContentfulAlert>();
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public bool LiveChatVisible { get; set; } = false;
        public LiveChat LiveChatText { get; set; } = new LiveChat("","")
        {
            Sys = new SystemProperties {Type = "Entry"}
        };
    }
}