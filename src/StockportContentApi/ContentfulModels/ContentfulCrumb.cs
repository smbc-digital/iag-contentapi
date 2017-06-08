using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(ExtensionJsonConverter))]
    public class ContentfulCrumb
    {
        // READ ME ----------------------------------
        // THE PROBLEM SEEMS TO BE WITHIN HERE, FOR SOME REASON WHEN ANY CONTENT TYPE HAS A BREADCRUMB IT WILL THROW AN ERROR
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
        public List<ContentfulSubItem> SubItems { get; set; } = new List<ContentfulSubItem>();
        public List<ContentfulSubItem> SecondaryItems { get; set; } = new List<ContentfulSubItem>();
        public List<ContentfulSubItem> TertiaryItems { get; set; } = new List<ContentfulSubItem>();
    }
}