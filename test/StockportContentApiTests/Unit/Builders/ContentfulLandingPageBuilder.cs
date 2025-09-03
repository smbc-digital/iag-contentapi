namespace StockportContentApiTests.Unit.Builders;

public class ContentfulLandingPageBuilder
{
    private List<ContentfulReference> _pageSections = new()
    {
        new ContentfulReferenceBuilder().Build() 
    };

    private readonly List<ContentfulReference> _breadcrumbs = new()
    {
        new ContentfulReferenceBuilder().Build()
    };

    private readonly List<ContentfulAlert> _alerts = new()
    {
        new ContentfulAlertBuilder().Build()
    };

    public ContentfulLandingPage Build() =>
        new()
        {
            Slug = "slug",
            Title = "title",
            Subtitle = "subtitle",
            Teaser = "teaser",
            MetaDescription = "meta description",
            Icon = "icon",
            HeaderType = "header type",
            HeaderColourScheme = EColourScheme.None,
            PageSections = _pageSections,
            Breadcrumbs = _breadcrumbs,
            Alerts = _alerts
        };
}