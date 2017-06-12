using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulTopic : IContentfulSubItem
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public Asset BackgroundImage { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset"} };
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public SystemProperties Sys { get; set; }
        public List<IContentfulSubItem> SubItems { get; set; } = new List<IContentfulSubItem>();
        public List<IContentfulSubItem> SecondaryItems { get; set; } = new List<IContentfulSubItem>();
        public List<IContentfulSubItem> TertiaryItems { get; set; } = new List<IContentfulSubItem>();
        public List<ContentfulCrumb> Breadcrumbs { get; set; } = new List<ContentfulCrumb>();
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public bool EmailAlerts { get; set; } = false;
        public string EmailAlertsTopicId { get; set; } = string.Empty;
        public ContentfulEventBanner EventBanner { get; set; } = new ContentfulEventBanner
        {
            Sys = new SystemProperties { Type = "Entry" }
        };
       public string Title { get; set; }
    }
}
