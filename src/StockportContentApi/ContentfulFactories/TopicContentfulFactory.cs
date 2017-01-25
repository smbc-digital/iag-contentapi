using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

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
            var subItems = entry.SubItems.Select(subItem => _subItemFactory.ToModel(subItem)).ToList();
            var secondaryItems = entry.SecondaryItems.Select(subItem => _subItemFactory.ToModel(subItem)).ToList();
            var tertiaryItems = entry.TertiaryItems.Select(subItem => _subItemFactory.ToModel(subItem)).ToList();
            var breadcrumbs = entry.Breadcrumbs.Select(crumb => _crumbFactory.ToModel(crumb)).ToList();
            var alerts = entry.Alerts.Select(alert => alert.Fields);
            var backgroundImage = entry.BackgroundImage.File.Url;

            return new Topic(entry.Slug, entry.Name, entry.Teaser, entry.Summary, entry.Icon, backgroundImage, 
                subItems, secondaryItems, tertiaryItems, breadcrumbs, alerts, entry.SunriseDate, entry.SunsetDate, 
                entry.EmailAlerts, entry.EmailAlertsTopicId);
        }
    }
}