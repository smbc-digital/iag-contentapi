using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class PaymentContentfulFactory : IContentfulFactory<ContentfulPayment, Payment>
    {
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;

        public PaymentContentfulFactory(IContentfulFactory<ContentfulAlert, Alert> alertFactory, ITimeProvider timeProvider, IContentfulFactory<ContentfulReference, Crumb> crumbFactory)
        {
            _alertFactory = alertFactory;
            _dateComparer = new DateComparer(timeProvider);
            _crumbFactory = crumbFactory;
        }

        public Payment ToModel(ContentfulPayment entry)
        {
            var alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                     .Select(alert => _alertFactory.ToModel(alert));

            var breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                              .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            return new Payment(entry.Title,
                entry.Slug,
                entry.Teaser,
                entry.Description,
                entry.PaymentDetailsText,
                entry.ReferenceLabel,
                entry.ParisReference,
                entry.Fund,
                entry.GlCodeCostCentreNumber,
                entry.Icon,
                breadcrumbs,
                entry.ReferenceValidation,
                entry.MetaDescription,
                entry.ReturnUrl,
                entry.CatalogueId,
                entry.AccountReference,
                entry.PaymentDescription,
                alerts
                );
        }
    }
}