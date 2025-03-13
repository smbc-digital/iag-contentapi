namespace StockportContentApi.ContentfulFactories;

public class ServicePayPaymentContentfulFactory(IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                                ITimeProvider timeProvider,
                                                IContentfulFactory<ContentfulReference, Crumb> crumbFactory) : IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment>
{
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory = crumbFactory;

    public ServicePayPayment ToModel(ContentfulServicePayPayment entry)
    {
        IEnumerable<Alert> alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                        .Where(alert => !alert.Severity.Equals("Condolence"))
                                        .Select(_alertFactory.ToModel);

        List<Crumb> breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                    .Select(_crumbFactory.ToModel).ToList();

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
            alerts);
    }
}