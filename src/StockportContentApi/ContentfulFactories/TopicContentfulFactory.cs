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

        public TopicContentfulFactory(IContentfulFactory<Entry<ContentfulSubItem>, SubItem> subItemFactory, IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory)
        {
            _subItemFactory = subItemFactory;
            _crumbFactory = crumbFactory;
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
            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.SystemProperties))
                                     .Select(alert => alert.Fields);
            var backgroundImage = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties) 
                                        ? entry.BackgroundImage.File.Url : string.Empty;

            return new Topic(entry.Slug, entry.Name, entry.Teaser, entry.Summary, entry.Icon, backgroundImage, 
                subItems, secondaryItems, tertiaryItems, breadcrumbs, alerts, entry.SunriseDate, entry.SunsetDate, 
                entry.EmailAlerts, entry.EmailAlertsTopicId);
        }
    }
}