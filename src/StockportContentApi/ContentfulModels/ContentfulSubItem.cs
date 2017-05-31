using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Newtonsoft.Json;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulSubItem
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public List<Entry<ContentfulSubItem>> SubItems { get; set; } = new List<Entry<ContentfulSubItem>>();
        public List<Entry<ContentfulSubItem>> SecondaryItems { get; set; } = new List<Entry<ContentfulSubItem>>();
        public List<Entry<ContentfulSubItem>> TertiaryItems { get; set; } = new List<Entry<ContentfulSubItem>>();
    }
}