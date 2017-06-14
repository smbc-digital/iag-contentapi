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
            var type = entry.Sys.ContentType.SystemProperties.Id == "startPage" 
                ? "start-page" 
                : entry.Sys.ContentType.SystemProperties.Id;
            var title = !string.IsNullOrEmpty(entry.Title) ? entry.Title : entry.Name;

            var image = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                                       ? entry.Image.File.Url : string.Empty;

            // TODO: FIX STACK OVERFLOW

            // build all of the sub items (only avaliable for topics)
            //var subItems = entry.SubItems != null
            //    ? entry.SubItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
            //    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
            //    .Select(item => ToModel(item)).ToList()
            //    : new List<SubItem>();

            //var secondaryItems = entry.SecondaryItems != null
            //    ? entry.SecondaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
            //    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
            //    .Select(item => ToModel(item)).ToList()
            //    : new List<SubItem>();

            //var tertiaryItems = entry.TertiaryItems != null
            //    ? entry.TertiaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
            //    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
            //    .Select(item => ToModel(item)).ToList()
            //    : new List<SubItem>();

            var allSubItems = new List<SubItem>();
            //allSubItems.AddRange(subItems);
            //allSubItems.AddRange(secondaryItems);
            //allSubItems.AddRange(tertiaryItems);

            return new SubItem(entry.Slug, title, entry.Teaser, 
                entry.Icon, type, entry.SunriseDate, entry.SunsetDate, image, allSubItems);
        }
    }
}