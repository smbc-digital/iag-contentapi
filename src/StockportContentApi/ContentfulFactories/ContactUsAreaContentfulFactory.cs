﻿using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class ContactUsAreaContentfulFactory : IContentfulFactory<ContentfulContactUsArea, ContactUsArea>
    {
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IContentfulFactory<ContentfulInsetText, InsetText> _insetTextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactUsAreaContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory, IHttpContextAccessor httpContextAccessor, IContentfulFactory<ContentfulReference, Crumb> crumbFactory, ITimeProvider timeProvider, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulInsetText, InsetText> insetTextFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _subitemFactory = subitemFactory;
            _crumbFactory = crumbFactory;
            _dateComparer = new DateComparer(timeProvider);
            _alertFactory = alertFactory;
            _insetTextFactory = insetTextFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public ContactUsArea ToModel(ContentfulContactUsArea entry)
        {
            var title = !string.IsNullOrEmpty(entry.Title)
                ? entry.Title
                : "";

            var slug = !string.IsNullOrEmpty(entry.Slug)
                ? entry.Slug
                : "";

            var breadcrumbs =
                entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                    .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var primaryItems =
                entry.PrimaryItems.Where(primItem => ContentfulHelpers.EntryIsNotALink(primItem.Sys)
                                                     && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(primItem.SunriseDate, primItem.SunsetDate))
                    .Select(item => _subitemFactory.ToModel(item)).ToList();

            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) &&
                                                     _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                .Select(alert => _alertFactory.ToModel(alert)).ToList();

            var insetTexts = entry.InsetText.Where(insetText => ContentfulHelpers.EntryIsNotALink(insetText.Sys))
                .Select(insetText => _insetTextFactory.ToModel(insetText)).ToList();

            return new ContactUsArea(slug, title, breadcrumbs, alerts, insetTexts, primaryItems).StripData(_httpContextAccessor);
        }
    }
}