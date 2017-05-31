using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
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