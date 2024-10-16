namespace StockportContentApi.ContentfulFactories;

public class ContactUsAreaContentfulFactory : IContentfulFactory<ContentfulContactUsArea, ContactUsArea>
{
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory> _contactUsCategoryFactory;

    public ContactUsAreaContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
                                        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
                                        ITimeProvider timeProvider,
                                        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                        IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory> contactUsCategoryFactory)
    {
        _subitemFactory = subitemFactory;
        _crumbFactory = crumbFactory;
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
        _contactUsCategoryFactory = contactUsCategoryFactory;
    }

    public ContactUsArea ToModel(ContentfulContactUsArea entry)
    {
        string title = !string.IsNullOrEmpty(entry.Title)
            ? entry.Title
            : string.Empty;

        string slug = !string.IsNullOrEmpty(entry.Slug)
            ? entry.Slug
            : string.Empty;

        string categoriesTitle = !string.IsNullOrEmpty(entry.CategoriesTitle)
            ? entry.CategoriesTitle
            : string.Empty;

        string insetTextTitle = !string.IsNullOrEmpty(entry.InsetTextTitle)
            ? entry.InsetTextTitle
            : string.Empty;

        string insetTextBody = !string.IsNullOrEmpty(entry.InsetTextBody)
            ? entry.InsetTextBody
            : string.Empty;

        List<Crumb> breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                    .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

        List<SubItem> primaryItems = entry.PrimaryItems.Where(primItem => ContentfulHelpers.EntryIsNotALink(primItem.Sys) 
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(primItem.SunriseDate, primItem.SunsetDate))
                                        .Select(item => _subitemFactory.ToModel(item)).ToList();

        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) 
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(alert => _alertFactory.ToModel(alert)).ToList();

        List<ContactUsCategory> contactUsCategories = entry.ContactUsCategories.Where(contactUsCategory => ContentfulHelpers.EntryIsNotALink(contactUsCategory.Sys))
                                                        .Select(contactUsCategory => _contactUsCategoryFactory.ToModel(contactUsCategory)).ToList();

        return new ContactUsArea(slug, title, categoriesTitle, breadcrumbs, alerts, primaryItems, contactUsCategories, insetTextTitle, insetTextBody, entry.MetaDescription);
    }
}