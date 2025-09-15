namespace StockportContentApiTests.Unit.Builders;

public class ContentfulContactUsAreaBuilder
{
    private string _slug = "slug";

    private List<ContentfulReference> _primaryItems = new();

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
            Title = "title",
            Slug = _slug,
            Breadcrumbs = _breadcrumbs,
            Alerts = _alerts,
            InsetTextTitle = "insetTextTitle",
            InsetTextBody = "insetTextBody",
            PrimaryItems = _primaryItems,
            ContactUsCategories = _contactUsCategories,
            MetaDescription = "metaDescription"
        };

    public ContentfulContactUsAreaBuilder Slug(string slug)
    {
        _slug = slug;
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

    public ContentfulContactUsAreaBuilder ContentfulContactUsCategories(List<ContentfulContactUsCategory> contactUsCategories)
    {
        _contactUsCategories = contactUsCategories;
        return this;
    }
}