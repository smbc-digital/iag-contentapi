namespace StockportContentApi.ContentfulFactories;

public class ContactUsAreaContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
                                            IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
                                            ITimeProvider timeProvider,
                                            IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                            IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory> contactUsCategoryFactory) : IContentfulFactory<ContentfulContactUsArea, ContactUsArea>
{
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory = crumbFactory;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory = subitemFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory> _contactUsCategoryFactory = contactUsCategoryFactory;

    public ContactUsArea ToModel(ContentfulContactUsArea entry)
    {
        string title = !string.IsNullOrEmpty(entry.Title)
            ? entry.Title
            : string.Empty;

        string slug = !string.IsNullOrEmpty(entry.Slug)
            ? entry.Slug
            : string.Empty;

        string insetTextTitle = !string.IsNullOrEmpty(entry.InsetTextTitle)
            ? entry.InsetTextTitle
            : string.Empty;

        string insetTextBody = !string.IsNullOrEmpty(entry.InsetTextBody)
            ? entry.InsetTextBody
            : string.Empty;

        List<Crumb> breadcrumbs = entry.Breadcrumbs.Where(breadcrumb => ContentfulHelpers.EntryIsNotALink(breadcrumb.Sys))
                                    .Select(_crumbFactory.ToModel).ToList();

        List<SubItem> primaryItems = entry.PrimaryItems.Where(primItem => ContentfulHelpers.EntryIsNotALink(primItem.Sys) 
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(primItem.SunriseDate, primItem.SunsetDate))
                                        .Select(_subitemFactory.ToModel).ToList();

        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) 
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(_alertFactory.ToModel).ToList();

        List<ContactUsCategory> contactUsCategories = entry.ContactUsCategories.Where(contactUsCategory => ContentfulHelpers.EntryIsNotALink(contactUsCategory.Sys))
                                                        .Select(_contactUsCategoryFactory.ToModel).ToList();

        return new ContactUsArea(slug, title, breadcrumbs, alerts, primaryItems, contactUsCategories, insetTextTitle, insetTextBody, entry.MetaDescription);
    }
}