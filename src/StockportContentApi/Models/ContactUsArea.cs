namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class ContactUsArea(string slug,
                        string title,
                        IEnumerable<Crumb> breadcrumbs,
                        IEnumerable<Alert> alerts,
                        IEnumerable<SubItem> primaryItems,
                        IEnumerable<ContactUsCategory> contactUsCategories,
                        string insetTextTitle,
                        string insetTextBody,
                        string metaDescription)
{
    public string Title { get; set; } = title;
    public string Slug { get; set; } = slug;
    public string Teaser { get; set; }
    public string Body { get; set; }
    public IEnumerable<SubItem> PrimaryItems { get; set; } = primaryItems;
    public IEnumerable<Crumb> Breadcrumbs { get; set; } = breadcrumbs;
    public IEnumerable<Alert> Alerts { get; } = alerts;
    public string InsetTextTitle { get; set; } = insetTextTitle;
    public string InsetTextBody { get; set; } = insetTextBody;
    public IEnumerable<ContactUsCategory> ContactUsCategories { get; set; } = contactUsCategories;
    public string MetaDescription { get; set; } = metaDescription;
}