﻿using System;
using System.Collections.Generic;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class TopicFactory : IFactory<Topic>
    {
        private readonly IBuildContentTypesFromReferences<Alert> _alertListFactory;
        private readonly IBuildContentTypesFromReferences<SubItem> _subitemFactory;
        private readonly IBuildContentTypesFromReferences<Crumb> _breadcrumbFactory;

        public TopicFactory(IBuildContentTypesFromReferences<Alert> alertListFactory, IBuildContentTypesFromReferences<SubItem> subitemFactory, IBuildContentTypesFromReferences<Crumb> breadcrumbFactory)
        {
            _alertListFactory = alertListFactory;
            _subitemFactory = subitemFactory;
            _breadcrumbFactory = breadcrumbFactory;
        }

        public Topic Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            dynamic fields = entry.fields;
            var slug = (string)fields.slug ?? string.Empty;
            var name = (string)fields.name ?? string.Empty;
            var teaser = (string)fields.teaser ?? string.Empty;
            var summary = (string)fields.summary ?? string.Empty;
            var icon = (string)fields.icon ?? string.Empty;

            DateTime sunriseDate = MakeMinValueDateIfNull(fields.sunriseDate);
            DateTime sunsetDate = MakeMinValueDateIfNull(fields.sunsetDate);

            var backgroundImage = contentfulResponse.GetImageUrl(fields.backgroundImage);
            var breadcrumbs = _breadcrumbFactory.BuildFromReferences(fields.breadcrumbs, contentfulResponse);
            var alerts = _alertListFactory.BuildFromReferences(fields.alerts, contentfulResponse);

            var subItems = _subitemFactory.BuildFromReferences(fields.subItems, contentfulResponse);
            var secondaryItems = _subitemFactory.BuildFromReferences(fields.secondaryItems, contentfulResponse);
            var tertiaryItems = _subitemFactory.BuildFromReferences(fields.tertiaryItems, contentfulResponse);

            DateTime outSunriseDate;
            DateTime.TryParse(sunriseDate.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? string.Empty, out outSunriseDate);
            DateTime outSunsetDate;
            DateTime.TryParse(sunsetDate.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? string.Empty, out outSunsetDate);

            var emailAlerts = false;
            if (entry.fields.emailAlerts != null) bool.TryParse((string)entry.fields.emailAlerts, out emailAlerts);
            var emailAlertsTopicId = (string)entry.fields.emailAlertsTopicId ?? string.Empty;

            return new Topic(slug, name, teaser, summary, icon, backgroundImage, subItems, secondaryItems, tertiaryItems, breadcrumbs, alerts, outSunriseDate, outSunsetDate, emailAlerts, emailAlertsTopicId);
        }

        private DateTime MakeMinValueDateIfNull(dynamic sunrisedate)
        {
            return sunrisedate ?? DateTime.MinValue;
        }
    }
}