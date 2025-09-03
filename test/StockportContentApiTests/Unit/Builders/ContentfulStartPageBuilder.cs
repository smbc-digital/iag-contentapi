namespace StockportContentApiTests.Unit.Builders;

public class ContentfulStartPageBuilder
{
    private string _slug = "startPageSlug";
    private readonly Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();

    private readonly List<ContentfulReference> _breadcrumbs = new()
    {
        new ContentfulReferenceBuilder().Build()
    };

    private readonly List<ContentfulAlert> _alerts = new()
    {
        new ContentfulAlertBuilder().Build(),
        new ContentfulAlertBuilder().WithSeverity("Condolence").Build()
    };

    public ContentfulStartPage Build()
        => new()
        {
            Title = "Start Page",
            Slug = _slug,
            Teaser = "this is a teaser",
            Summary = "This is a summary",
            UpperBody = "An upper body",
            FormLink = "http://start.com",
            LowerBody = "Lower body",
            BackgroundImage = _image,
            Icon = "icon",
            Breadcrumbs = _breadcrumbs,
            Alerts = _alerts,
            AlertsInline = _alerts
        };

    public ContentfulStartPageBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }
}