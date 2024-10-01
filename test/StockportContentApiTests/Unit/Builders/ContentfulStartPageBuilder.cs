namespace StockportContentApiTests.Builders;

public class ContentfulStartPageBuilder
{
    private string _title { get; set; } = "Start Page";
    private string _slug { get; set; } = "startPageSlug";
    private string _teaser { get; set; } = "this is a teaser";
    private string _summary { get; set; } = "This is a summary";
    private string _upperBody { get; set; } = "An upper body";
    private string _formLinkLabel { get; set; } = "Start now";
    private string _formLink { get; set; } = "http://start.com";
    private string _lowerBody { get; set; } = "Lower body";
    private readonly Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
    private string _icon { get; set; } = "icon";

    private readonly List<ContentfulReference> _breadcrumbs = new()
    {
      new ContentfulReferenceBuilder().Build()
    };

    private readonly List<ContentfulAlert> _alerts = new()
    {
        new ContentfulAlertBuilder().Build()
    };

    public ContentfulStartPage Build()
    {
        return new ContentfulStartPage()
        {
            Title = _title,
            Slug = _slug,
            Teaser = _teaser,
            Summary = _summary,
            UpperBody = _upperBody,
            FormLinkLabel = _formLinkLabel,
            FormLink = _formLink,
            LowerBody = _lowerBody,
            BackgroundImage = _image,
            Icon = _icon,
            Breadcrumbs = _breadcrumbs,
            Alerts = _alerts
        };
    }

    public ContentfulStartPageBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulStartPageBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulStartPageBuilder Teaser(string teaser)
    {
        _teaser = teaser;
        return this;
    }
}