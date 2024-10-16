namespace StockportContentApiTests.Unit.Builders;

public class ContentfulLandingPageBuilder
{
    private readonly string _slug = "slug";
    private readonly string _title = "title";
    private readonly string _subtitle = "subtitle";
    private readonly string _teaser = "teaser";
    private readonly string _metaDescription = "metaDescription";
    private readonly string _icon = "icon";
    private readonly string _headerType = "headerType";
    private readonly EColourScheme _headerColourScheme = EColourScheme.None;
    private readonly List<ContentfulReference> _pageSections = new();

    public ContentfulLandingPage Build() =>
        new()
        {
            Slug = _slug,
            Title = _title,
            Subtitle = _subtitle,
            Teaser = _teaser,
            MetaDescription = _metaDescription,
            Icon = _icon,
            HeaderType = _headerType,
            HeaderColourScheme = _headerColourScheme,
            PageSections = _pageSections
        };
}