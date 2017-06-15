using System;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class TopicFactory : IFactory<Topic>
    {
        private readonly IBuildContentTypesFromReferences<Alert> _alertListFactory;
        private readonly IBuildContentTypesFromReferences<SubItem> _subitemFactory;
        private readonly IBuildContentTypesFromReferences<Crumb> _breadcrumbFactory;
        private readonly IBuildContentTypeFromReference<EventBanner> _eventBannerFactory;
        private readonly IBuildContentTypesFromReferences<ExpandingLinkBox> _expandingLinkBoxFactory;

        public TopicFactory(IBuildContentTypesFromReferences<Alert> alertListFactory, 
                            IBuildContentTypesFromReferences<SubItem> subitemFactory, 
                            IBuildContentTypesFromReferences<Crumb> breadcrumbFactory,
                            IBuildContentTypeFromReference<EventBanner> eventBannerFactory,
                            IBuildContentTypesFromReferences<ExpandingLinkBox> expandingLinkBoxFactory)
        {
            _alertListFactory = alertListFactory;
            _subitemFactory = subitemFactory;
            _breadcrumbFactory = breadcrumbFactory;
            _eventBannerFactory = eventBannerFactory;
            _expandingLinkBoxFactory = expandingLinkBoxFactory;
        }

        public Topic Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            dynamic fields = entry.fields;
            var slug = (string)fields.slug ?? string.Empty;
            var name = (string)fields.name ?? string.Empty;
            var teaser = (string)fields.teaser ?? string.Empty;
            var summary = (string)fields.summary ?? string.Empty;
            var icon = (string)fields.icon ?? string.Empty;
            var backgroundImage = contentfulResponse.GetImageUrl(fields.backgroundImage);
            var image = contentfulResponse.GetImageUrl(fields.image);
            var breadcrumbs = _breadcrumbFactory.BuildFromReferences(fields.breadcrumbs, contentfulResponse);
            var alerts = _alertListFactory.BuildFromReferences(fields.alerts, contentfulResponse);
            var subItems = _subitemFactory.BuildFromReferences(fields.subItems, contentfulResponse);
            var secondaryItems = _subitemFactory.BuildFromReferences(fields.secondaryItems, contentfulResponse);
            var tertiaryItems = _subitemFactory.BuildFromReferences(fields.tertiaryItems, contentfulResponse);
            DateTime sunriseDate = DateComparer.DateFieldToDate(fields.sunriseDate);
            DateTime sunsetDate = DateComparer.DateFieldToDate(fields.sunsetDate);
            var emailAlerts = false;
            if (entry.fields.emailAlerts != null) bool.TryParse((string)entry.fields.emailAlerts, out emailAlerts);
            var emailAlertsTopicId = (string)entry.fields.emailAlertsTopicId ?? string.Empty;
            var eventBanner = _eventBannerFactory.BuildFromReference(fields.eventBanner, contentfulResponse);
            var expandingLinkTitle = (string)fields.expandingLinkTitle ?? string.Empty;
            var expandingLinkBoxes = _expandingLinkBoxFactory.BuildFromReferences(fields.expandingLinkBoxes, contentfulResponse);

            return new Topic(slug, name, teaser, summary, icon, backgroundImage, image, subItems, secondaryItems,
                tertiaryItems, breadcrumbs, alerts, sunriseDate, sunsetDate, emailAlerts, emailAlertsTopicId, eventBanner, expandingLinkTitle, expandingLinkBoxes);
        }
    }
}