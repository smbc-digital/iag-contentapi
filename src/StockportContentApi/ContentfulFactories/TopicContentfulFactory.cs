using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class TopicContentfulFactory : IContentfulFactory<ContentfulTopic, Topic>
    {
        private readonly IContentfulFactory<Entry<ContentfulSubItem>, SubItem> _subItemFactory;
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;
        private readonly IContentfulFactory<Entry<ContentfulAlert>, Alert> _alertFactory;
        private readonly DateComparer _dateComparer;

        public TopicContentfulFactory(IContentfulFactory<Entry<ContentfulSubItem>, SubItem> subItemFactory, IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory, IContentfulFactory<Entry<ContentfulAlert>, Alert> alertFactory, ITimeProvider timeProvider)
        {
            _subItemFactory = subItemFactory;
            _crumbFactory = crumbFactory;
            _alertFactory = alertFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public Topic ToModel(ContentfulTopic entry)
        {
            var subItems = entry.SubItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.SystemProperties))
                                         .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();
            var secondaryItems = entry.SecondaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.SystemProperties))
                                                     .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();
            var tertiaryItems = entry.TertiaryItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.SystemProperties))
                                                   .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();
            var breadcrumbs = entry.Breadcrumbs.Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.SystemProperties))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.SystemProperties)
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.Fields.SunriseDate, alert.Fields.SunsetDate))
                                     .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var backgroundImage = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties) 
                                        ? entry.BackgroundImage.File.Url : string.Empty;

            var image = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                                        ? entry.Image.File.Url : string.Empty;

            return new Topic(entry.Slug, entry.Name, entry.Teaser, entry.Summary, entry.Icon, backgroundImage, image,
                subItems, secondaryItems, tertiaryItems, breadcrumbs, alerts, entry.SunriseDate, entry.SunsetDate, 
                entry.EmailAlerts, entry.EmailAlertsTopicId);
        }
    }
}