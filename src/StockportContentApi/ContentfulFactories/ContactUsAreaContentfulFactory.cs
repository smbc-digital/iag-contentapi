namespace StockportContentApi.ContentfulFactories;

public interface IContactUsAreaContentfulFactory : IContentfulFactory<ContentfulContactUsArea, ContactUsArea>
{
    ContactUsArea ToCacheModel(ContentfulContactUsArea entry);
    ContactUsArea FilterForCurrentDate(ContactUsArea entry);
}

public class ContactUsAreaContentfulFactory(ISubItemContentfulFactory subitemFactory,
                                            IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
                                            ITimeProvider timeProvider,
                                            IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                            IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory> contactUsCategoryFactory) : IContactUsAreaContentfulFactory
{
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory = crumbFactory;
    private readonly ISubItemContentfulFactory _subitemFactory = subitemFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory> _contactUsCategoryFactory = contactUsCategoryFactory;

    public ContactUsArea ToModel(ContentfulContactUsArea entry) =>
        ToModel(entry, includeInactiveItems: false);

    public ContactUsArea ToCacheModel(ContentfulContactUsArea entry) =>
        ToModel(entry, includeInactiveItems: true);

    private ContactUsArea ToModel(ContentfulContactUsArea entry, bool includeInactiveItems)
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
                                            && (includeInactiveItems || _dateComparer.DateNowIsWithinSunriseAndSunsetDates(primItem.SunriseDate, primItem.SunsetDate)))
                                        .Select(primItem => includeInactiveItems
                                            ? _subitemFactory.ToCacheModel(primItem)
                                            : _subitemFactory.ToModel(primItem))
                                        .ToList();

        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) 
                                    && (includeInactiveItems || _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate)))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(_alertFactory.ToModel).ToList();

        List<ContactUsCategory> contactUsCategories = entry.ContactUsCategories.Where(contactUsCategory => ContentfulHelpers.EntryIsNotALink(contactUsCategory.Sys))
                                                        .Select(_contactUsCategoryFactory.ToModel).ToList();

        return new ContactUsArea(slug, title, breadcrumbs, alerts, primaryItems, contactUsCategories, insetTextTitle, insetTextBody, entry.MetaDescription);
    }

    public ContactUsArea FilterForCurrentDate(ContactUsArea entry)
    {
        List<SubItem> primaryItems = entry.PrimaryItems
            .Where(item => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(item.SunriseDate, item.SunsetDate))
            .Select(_subitemFactory.FilterForCurrentDate)
            .ToList();
        List<Alert> alerts = entry.Alerts
            .Where(alert => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
            .ToList();

        return new ContactUsArea(entry.Slug,
            entry.Title,
            entry.Breadcrumbs,
            alerts,
            primaryItems,
            entry.ContactUsCategories,
            entry.InsetTextTitle,
            entry.InsetTextBody,
            entry.MetaDescription)
        {
            Teaser = entry.Teaser,
            Body = entry.Body
        };
    }

}