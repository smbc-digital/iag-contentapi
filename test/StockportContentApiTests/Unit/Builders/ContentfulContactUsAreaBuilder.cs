namespace StockportContentApiTests.Builders;

public class ContentfulContactUsAreaBuilder
{
    private string _title { get; set; } = "title";
    private string _slug { get; set; } = "slug";
    private string _categoriesTitle { get; set; } = "categoriesTitle";
    private string _metaDescription { get; set; } = "metaDescription";
    private string _insetTextTitle { get; set; } = "insetTextTitle";
    private string _insetTextBody { get; set; } = "insetTextBody";

    private List<ContentfulReference> _primaryItems { get; set; } = new List<ContentfulReference>();

    private List<ContentfulReference> _breadcrumbs = new()
    {
      new ContentfulReferenceBuilder().Build()
    };

    private List<ContentfulAlert> _alerts = new()
    {
        new ContentfulAlertBuilder().Build()
    };

    private List<ContentfulContactUsCategory> _contactUsCategories = new()
    {
        new ContentfulContactUsCategoryBuilder().Build()
    };

    public ContentfulContactUsArea Build()
        => new()
        {
            Title = _title,
            Slug = _slug,
            CategoriesTitle = _categoriesTitle,
            Breadcrumbs = _breadcrumbs,
            Alerts = _alerts,
            InsetTextTitle = _insetTextTitle,
            InsetTextBody = _insetTextBody,
            PrimaryItems = _primaryItems,
            ContactUsCategories = _contactUsCategories,
            MetaDescription = _metaDescription
        };

    public ContentfulContactUsAreaBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulContactUsAreaBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulContactUsAreaBuilder CategoriesTitle(string categoriesTitle)
    {
        _categoriesTitle = categoriesTitle;
        return this;
    }

    public ContentfulContactUsAreaBuilder Breadcrumbs(List<ContentfulReference> breadcrumbs)
    {
        _breadcrumbs = breadcrumbs;
        return this;
    }

    public ContentfulContactUsAreaBuilder PrimaryItems(List<ContentfulReference> primaryItems)
    {
        _primaryItems = primaryItems;
        return this;
    }

    public ContentfulContactUsAreaBuilder Alerts(List<ContentfulAlert> alerts)
    {
        _alerts = alerts;
        return this;
    }

    public ContentfulContactUsAreaBuilder InsetTextTitle(string insetTextTitle)
    {
        _insetTextTitle = insetTextTitle;
        return this;
    }

    public ContentfulContactUsAreaBuilder InsetTextBody(string insetTextBody)
    {
        _insetTextTitle = insetTextBody;
        return this;
    }

    public ContentfulContactUsAreaBuilder ContentfulContactUsCategories(List<ContentfulContactUsCategory> contactUsCategories)
    {
        _contactUsCategories = contactUsCategories;
        return this;
    }
}