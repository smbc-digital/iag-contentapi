namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class ContactUsArea
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Teaser { get; set; }
    public string Body { get; set; }
    public IEnumerable<SubItem> PrimaryItems { get; set; }
    public IEnumerable<Crumb> Breadcrumbs { get; set; }
    public IEnumerable<Alert> Alerts { get; }
    public string InsetTextTitle { get; set; }
    public string InsetTextBody { get; set; }
    public IEnumerable<ContactUsCategory> ContactUsCategories { get; set; }
    public string MetaDescription { get; set; }

    public ContactUsArea(string slug,
        string title,
        IEnumerable<Crumb> breadcrumbs,
        IEnumerable<Alert> alerts,
        IEnumerable<SubItem> primaryItems,
        IEnumerable<ContactUsCategory> contactUsCategories,
        string insetTextTitle,
        string insetTextBody,
        string metaDescription)
    {
        Title = title;
        Slug = slug;
        Breadcrumbs = breadcrumbs;
        Alerts = alerts;
        PrimaryItems = primaryItems;
        ContactUsCategories = contactUsCategories;
        InsetTextTitle = insetTextTitle;
        InsetTextBody = insetTextBody;
        MetaDescription = metaDescription;
    }
}