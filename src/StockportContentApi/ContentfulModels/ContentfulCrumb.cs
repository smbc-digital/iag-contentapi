using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulCrumb
    {
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public List<Entry<ContentfulSubItem>> SubItems { get; set; } = new List<Entry<ContentfulSubItem>>();
        public List<Entry<ContentfulSubItem>> SecondaryItems { get; set; } = new List<Entry<ContentfulSubItem>>();
        public List<Entry<ContentfulSubItem>> TertiaryItems { get; set; } = new List<Entry<ContentfulSubItem>>();
    }
}