using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    
    public class ContentfulCrumb
    {
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
        public List<ContentfulSubItem> SubItems { get; set; } = new List<ContentfulSubItem>();
        public List<ContentfulSubItem> SecondaryItems { get; set; } = new List<ContentfulSubItem>();
        public List<ContentfulSubItem> TertiaryItems { get; set; } = new List<ContentfulSubItem>();
    }
}