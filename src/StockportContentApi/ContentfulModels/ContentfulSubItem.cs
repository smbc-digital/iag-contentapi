using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.ContentfulModels
{

    public interface IContentfulSubItem
    {
        string Icon { get; set; }
        Asset Image { get; set; }
        string Name { get; set; }
        List<IContentfulSubItem> SecondaryItems { get; set; }
        string Slug { get; set; }
        List<IContentfulSubItem> SubItems { get; set; }
        DateTime SunriseDate { get; set; }
        DateTime SunsetDate { get; set; }
        SystemProperties Sys { get; set; }
        string Teaser { get; set; }
        List<IContentfulSubItem> TertiaryItems { get; set; }
        string Title { get; set; }
    }

    public class ContentfulSubItem : IContentfulSubItem
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public List<IContentfulSubItem> SubItems { get; set; }
        public List<IContentfulSubItem> SecondaryItems { get; set; }
        public List<IContentfulSubItem> TertiaryItems { get; set; }
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}