using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ParentTopicContentfulFactory : IContentfulFactory<Entry<ContentfulCrumb>, Topic>
    {
        private readonly IContentfulFactory<Entry<ContentfulSubItem>, SubItem> _subItemFactory;
        private readonly DateComparer _dateComparer;

        public ParentTopicContentfulFactory(
            IContentfulFactory<Entry<ContentfulSubItem>, SubItem> subItemFactory, 
            ITimeProvider timeProvider)
        {
            _subItemFactory = subItemFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public Topic ToModel(Entry<ContentfulCrumb> entry)
        {
            var subItems = entry.Fields.SubItems.Where(subItem => subItem.Fields != null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))
                                         .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();
            var secondaryItems = entry.Fields.SecondaryItems.Select(subItem => _subItemFactory.ToModel(subItem)).ToList();
            var tertiaryItems = entry.Fields.TertiaryItems.Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

            return new Topic(entry.Fields.Name, entry.Fields.Slug, subItems, secondaryItems, tertiaryItems);
        }
    }
}