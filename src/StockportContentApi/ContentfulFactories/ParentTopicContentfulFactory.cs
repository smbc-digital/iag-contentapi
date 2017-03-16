using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ParentTopicContentfulFactory : IContentfulFactory<Entry<ContentfulArticle>, Topic>
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

        private Entry<ContentfulArticle> _entry;

        public Topic ToModel(Entry<ContentfulArticle> entry)
        {
            _entry = entry;

            var topicInBreadcrumb = entry.Fields.Breadcrumbs.LastOrDefault(o => o.SystemProperties.ContentType.SystemProperties.Id == "topic");

            if (topicInBreadcrumb == null) return new NullTopic();

            var subItems = topicInBreadcrumb.Fields.SubItems
                .Select(CheckCurrentArticle)
                .Where(subItem => subItem.Fields != null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))
                .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

            var secondaryItems = topicInBreadcrumb.Fields.SecondaryItems
                .Select(CheckCurrentArticle)
                .Where(subItem => subItem.Fields != null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))
                .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

            var tertiaryItems = topicInBreadcrumb.Fields.TertiaryItems
                .Select(CheckCurrentArticle)
                .Where(subItem => subItem.Fields != null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))
                .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

            return new Topic(topicInBreadcrumb.Fields.Name, topicInBreadcrumb.Fields.Slug, subItems, secondaryItems, tertiaryItems);
        }

        private Entry<ContentfulSubItem> CheckCurrentArticle(Entry<ContentfulSubItem> item)
        {
            if (item.SystemProperties.Id != _entry.SystemProperties.Id) return item;

            // the link is to the current article
            item.Fields = new ContentfulSubItem
            {
                Icon = _entry.Fields.Icon,
                Title = _entry.Fields.Title,
                SunriseDate = _entry.Fields.SunriseDate,
                SunsetDate = _entry.Fields.SunsetDate,
                Slug = _entry.Fields.Slug,
                Image = _entry.Fields.Image,
                Teaser = _entry.Fields.Teaser
            };

            item.SystemProperties.ContentType = new ContentType() { SystemProperties = new SystemProperties() { Id = "article"} };

            return item;
        }
    }
}