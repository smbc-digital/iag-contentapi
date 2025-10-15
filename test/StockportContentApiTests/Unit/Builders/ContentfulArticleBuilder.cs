namespace StockportContentApiTests.Unit.Builders;

public class ContentfulArticleBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
    private string _body = "body";
    private List<ContentfulAlert> _alertsInline = new() { new ContentfulAlertBuilder().Build() };
    private List<ContentfulReference> _breadcrumbs = new() { new ContentfulReferenceBuilder().Build() };
    private List<ContentfulAlert> _alerts = new() { new ContentfulAlertBuilder().Build() };
    private string _systemId = "id";
    private  string _associatedTagCategory = "dance";
    public Asset Image { get => _image; set => _image = value; }

    private readonly List<ContentfulTrustedLogo> _articleBranding = new()
    {
        new ContentfulTrustedLogo()
        {
            Title = "branding title",
            Text = "branding text",
            Image = new Asset(),
            Link = "branding-url"
        }
    };

    private List<ContentfulReference> _relatedContent = new()
    {
        new ContentfulReferenceBuilder().Build()
    };

    public ContentfulArticle Build()
        => new()
        {
            Alerts = _alerts,
            BackgroundImage = new ContentfulAssetBuilder().Url("image-url.jpg").Build(),
            Body = _body,
            Breadcrumbs = _breadcrumbs,
            Documents = new() { new ContentfulDocumentBuilder().Build() },
            Icon = "icon",
            Profiles = new() { new ContentfulProfileBuilder().Build() },
            Slug = _slug,
            Title = _title,
            Teaser = "teaser",
            Sections = new() { new ContentfulSectionBuilder().Build() },
            SunriseDate = new(2016, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            SunsetDate = new(2017, 1, 20, 0, 0, 0, DateTimeKind.Utc),
            Image = Image,
            AlertsInline = _alertsInline,
            Sys = new SystemProperties
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } },
                Id = _systemId,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            },
            TrustedLogos = _articleBranding,
            RelatedContent = _relatedContent,
            AssociatedTagCategory = _associatedTagCategory,
            Websites = new List<string>() { "websiteId" }
        };

    public ContentfulArticleBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulArticleBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulArticleBuilder Breadcrumbs(List<ContentfulReference> breadcrumbs)
    {
        _breadcrumbs = breadcrumbs;
        return this;
    }

    public ContentfulArticleBuilder AlertsInline(List<ContentfulAlert> alertsInline)
    {
        _alertsInline = alertsInline;
        return this;
    }

    public ContentfulArticleBuilder Alerts(List<ContentfulAlert> alerts)
    {
        _alerts = alerts;
        return this;
    }

    public ContentfulArticleBuilder Body(string body)
    {
        _body = body;
        return this;
    }

    public ContentfulArticleBuilder SystemId(string id)
    {
        _systemId = id;
        return this;
    }

    public ContentfulArticleBuilder WithBreadcrumbContentType(string contentType)
    {
        if (_breadcrumbs.Any())
            _breadcrumbs[0].Sys.ContentType.SystemProperties.Id = contentType;

        return this;
    }

    public ContentfulArticleBuilder WithAssociatedTagCategory(string associatedTagCategory)
    {
        _associatedTagCategory = associatedTagCategory;
        return this;
    }
}