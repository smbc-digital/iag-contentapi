﻿using System.Collections.Generic;
using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class SubItemContentfulFactory : IContentfulFactory<ContentfulReference, SubItem>
    {
        private readonly DateComparer _dateComparer;

        public SubItemContentfulFactory(ITimeProvider timeProvider)
        {
            _dateComparer = new DateComparer(timeProvider);
        }

        public SubItem ToModel(ContentfulReference entry)
        {
            var type = GetEntryType(entry);
            var image = GetEntryImage(entry);
            var title = GetEntryTitle(entry);

            // build all of the sub items (only avaliable for topics)
            var subItems = new List<SubItem>();

            foreach (var item in entry.SubItems.Where(subItem => EntryIsValid(subItem)))
            {
                var newItem = new SubItem(item.Slug, GetEntryTitle(item), item.Teaser, item.Icon, GetEntryType(item), item.SunriseDate, item.SunsetDate, GetEntryImage(item), new List<SubItem>());
                subItems.Add(newItem);
            }

            foreach (var item in entry.SecondaryItems.Where(subItem => EntryIsValid(subItem)))
            {
                var newItem = new SubItem(item.Slug, GetEntryTitle(item), item.Teaser, item.Icon, GetEntryType(item), item.SunriseDate, item.SunsetDate, GetEntryImage(item), new List<SubItem>());
                subItems.Add(newItem);
            }

            foreach (var item in entry.TertiaryItems.Where(subItem => EntryIsValid(subItem)))
            {
                var newItem = new SubItem(item.Slug, GetEntryTitle(item), item.Teaser, item.Icon, GetEntryType(item), item.SunriseDate, item.SunsetDate, GetEntryImage(item), new List<SubItem>());
                subItems.Add(newItem);
            }

            return new SubItem(entry.Slug, title, entry.Teaser, 
                entry.Icon, type, entry.SunriseDate, entry.SunsetDate, image, subItems);
        }

        private string GetEntryType(ContentfulReference entry)
        {
            return entry.Sys.ContentType.SystemProperties.Id == "startPage" ? "start-page" : entry.Sys.ContentType.SystemProperties.Id;
        }

        private string GetEntryImage(ContentfulReference entry)
        {
            return ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;
        }

        private string GetEntryTitle(ContentfulReference entry)
        {
            return !string.IsNullOrEmpty(entry.Title) ? entry.Title : entry.Name;
        }

        private bool EntryIsValid(ContentfulReference entry)
        {
            return ContentfulHelpers.EntryIsNotALink(entry.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(entry.SunriseDate, entry.SunsetDate);
        }
    }
}