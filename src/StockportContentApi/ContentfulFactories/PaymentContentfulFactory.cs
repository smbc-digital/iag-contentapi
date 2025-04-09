namespace StockportContentApi.ContentfulFactories;

public class PaymentContentfulFactory(IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                    ITimeProvider timeProvider,
                                    IContentfulFactory<ContentfulReference, Crumb> crumbFactory) : IContentfulFactory<ContentfulPayment, Payment>
{
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory = crumbFactory;

    public Payment ToModel(ContentfulPayment entry)
    {
        IEnumerable<Alert> alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                        .Where(alert => !alert.Severity.Equals("Condolence"))
                                        .Select(_alertFactory.ToModel);

        List<Crumb> breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                    .Select(_crumbFactory.ToModel).ToList();

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
            alerts);
    }
}