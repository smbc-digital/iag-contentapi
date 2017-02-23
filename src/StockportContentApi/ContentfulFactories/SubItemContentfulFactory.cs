﻿using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class SubItemContentfulFactory : IContentfulFactory<Entry<ContentfulSubItem>, SubItem>
    {
        public SubItem ToModel(Entry<ContentfulSubItem> entry)
        {
            var type = entry.SystemProperties.ContentType.SystemProperties.Id == "startPage" 
                ? "start-page" 
                : entry.SystemProperties.ContentType.SystemProperties.Id;
            var title = !string.IsNullOrEmpty(entry.Fields.Title) ? entry.Fields.Title : entry.Fields.Name;

            return new SubItem(entry.Fields.Slug, title, entry.Fields.Teaser, 
                entry.Fields.Icon, type, entry.Fields.SunriseDate, entry.Fields.SunsetDate, entry.Fields.Image);
        }
    }
}