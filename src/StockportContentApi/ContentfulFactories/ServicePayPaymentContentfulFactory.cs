using System.Linq;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ServicePayPaymentContentfulFactory : IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment>
    {
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServicePayPaymentContentfulFactory(IContentfulFactory<ContentfulAlert, Alert> alertFactory, ITimeProvider timeProvider, IContentfulFactory<ContentfulReference, Crumb> crumbFactory, IHttpContextAccessor httpContextAccessor)
        {
            _alertFactory = alertFactory;
            _dateComparer = new DateComparer(timeProvider);
            _crumbFactory = crumbFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public ServicePayPayment ToModel(ContentfulServicePayPayment entry)
        {
            var alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                     .Select(alert => _alertFactory.ToModel(alert));

            var breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                              .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            return new ServicePayPayment(
                entry.Title,
                entry.Slug,
                entry.Teaser,
                entry.Description,
                entry.PaymentDetailsText,
                entry.ReferenceLabel,
                entry.Icon,
                breadcrumbs,
                entry.ReferenceValidation,
                entry.MetaDescription,
                entry.ReturnUrl,
                entry.CatalogueId,
                entry.AccountReference,
                entry.PaymentDescription,
                alerts,
                entry.PaymentAmount
                ).StripData(_httpContextAccessor);
        }
    }
}
