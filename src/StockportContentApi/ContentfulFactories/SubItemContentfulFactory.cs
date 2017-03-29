using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class SubItemContentfulFactory : IContentfulFactory<Entry<ContentfulSubItem>, SubItem>
    {

        private readonly DateComparer _dateComparer;

        public SubItemContentfulFactory(ITimeProvider timeProvider)
        {
            _dateComparer = new DateComparer(timeProvider);
        }

        public SubItem ToModel(Entry<ContentfulSubItem> entry)
        {
            var type = entry.SystemProperties.ContentType.SystemProperties.Id == "startPage" 
                ? "start-page" 
                : entry.SystemProperties.ContentType.SystemProperties.Id;
            var title = !string.IsNullOrEmpty(entry.Fields.Title) ? entry.Fields.Title : entry.Fields.Name;

            var image = ContentfulHelpers.EntryIsNotALink(entry.Fields.Image.SystemProperties)
                                       ? entry.Fields.Image.File.Url : string.Empty;

            // build all of the sub items (only avaliable for topics)
            var subItems = entry.Fields.SubItems != null
                ? entry.Fields.SubItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.SystemProperties)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))
                .Select(subItem => ToModel(subItem)).ToList()
                : new List<SubItem>();

            var secondaryItems = entry.Fields.SecondaryItems != null
                ? entry.Fields.SecondaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.SystemProperties)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))
                .Select(subItem => ToModel(subItem)).ToList()
                : new List<SubItem>();

            var tertiaryItems = entry.Fields.TertiaryItems != null
                ? entry.Fields.TertiaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.SystemProperties)
                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))
                .Select(subItem => ToModel(subItem)).ToList()
                : new List<SubItem>();

            var allSubItems = new List<SubItem>();
            allSubItems.AddRange(subItems);
            allSubItems.AddRange(secondaryItems);
            allSubItems.AddRange(tertiaryItems);

            return new SubItem(entry.Fields.Slug, title, entry.Fields.Teaser, 
                entry.Fields.Icon, type, entry.Fields.SunriseDate, entry.Fields.SunsetDate, image, allSubItems);
        }
    }
}