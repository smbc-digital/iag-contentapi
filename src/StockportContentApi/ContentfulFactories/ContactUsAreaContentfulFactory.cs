using System.Linq;
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
        private readonly IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory> _contactUsCategoryFactory;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactUsAreaContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory, IHttpContextAccessor httpContextAccessor, IContentfulFactory<ContentfulReference, Crumb> crumbFactory, ITimeProvider timeProvider, IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory> contactUsCategoryFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _subitemFactory = subitemFactory;
            _crumbFactory = crumbFactory;
            _dateComparer = new DateComparer(timeProvider);
            _alertFactory = alertFactory;
            _contactUsCategoryFactory = contactUsCategoryFactory;
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

            var categoriesTitle = !string.IsNullOrEmpty(entry.CategoriesTitle)
                ? entry.CategoriesTitle
                : "";

            var insetTextTitle = !string.IsNullOrEmpty(entry.InsetTextTitle)
                ? entry.InsetTextTitle
                : "";

            var insetTextBody = !string.IsNullOrEmpty(entry.InsetTextBody)
                ? entry.InsetTextBody
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

            var contactUsCategories =
                entry.ContactUsCategories.Where(contactUsCategory => ContentfulHelpers.EntryIsNotALink(contactUsCategory.Sys))
                    .Select(contactUsCategory => _contactUsCategoryFactory.ToModel(contactUsCategory)).ToList();

            return new ContactUsArea(slug, title, categoriesTitle, breadcrumbs, alerts, primaryItems, contactUsCategories, insetTextTitle, insetTextBody, entry.MetaDescription).StripData(_httpContextAccessor);
        }
    }
}