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
        private readonly IContentfulFactory<Entry<ContentfulEventBanner>, EventBanner> _eventBannerFactory;
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<Entry<ContentfulExpandingLinkBox>, ExpandingLinkBox> _expandingLinkBoxFactory;

        public TopicContentfulFactory(IContentfulFactory<Entry<ContentfulSubItem>, SubItem> subItemFactory, IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory, IContentfulFactory<Entry<ContentfulAlert>, Alert> alertFactory, IContentfulFactory<Entry<ContentfulEventBanner>, EventBanner> eventBannerFactory, IContentfulFactory<Entry<ContentfulExpandingLinkBox>, ExpandingLinkBox> expandingLinkBoxFactory, ITimeProvider timeProvider)
        {
            _subItemFactory = subItemFactory;
            _crumbFactory = crumbFactory;
            _alertFactory = alertFactory;
            _dateComparer = new DateComparer(timeProvider);
            _eventBannerFactory = eventBannerFactory;
            _expandingLinkBoxFactory = expandingLinkBoxFactory;
        }

        public Topic ToModel(ContentfulTopic entry)
        {
            var subItems = entry.SubItems.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.SystemProperties)
                                         && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.Fields.SunriseDate, subItem.Fields.SunsetDate))
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

            var eventBanner = ContentfulHelpers.EntryIsNotALink(entry.EventBanner.SystemProperties)
                                ? _eventBannerFactory.ToModel(entry.EventBanner) : new NullEventBanner();

            var expandingLinkBoxes =
                entry.ExpandingLinkBoxes.Where(e => ContentfulHelpers.EntryIsNotALink(e.SystemProperties))
                    .Select(e => _expandingLinkBoxFactory.ToModel(e)).ToList();

            return new Topic(entry.Slug, entry.Name, entry.Teaser, entry.Summary, entry.Icon, backgroundImage, image,
                subItems, secondaryItems, tertiaryItems, breadcrumbs, alerts, entry.SunriseDate, entry.SunsetDate, 
                entry.EmailAlerts, entry.EmailAlertsTopicId, eventBanner, entry.ExpandingLinkTitle, expandingLinkBoxes);
        }
    }
}