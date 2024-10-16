namespace StockportContentApiTests.Unit.Builders;

public class ContentfulArticleBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private readonly string _teaser = "teaser";
    private readonly string _icon = "icon";
    private readonly Asset _backgroundImage = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
    private Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
    private string _body = "body";
    private readonly DateTime _sunriseDate = new(2016, 1, 10, 0, 0, 0, DateTimeKind.Utc);
    private readonly DateTime _sunsetDate = new(2017, 1, 20, 0, 0, 0, DateTimeKind.Utc);
    private List<ContentfulAlert> _alertsInline = new() { new ContentfulAlertBuilder().Build() };
    private List<ContentfulReference> _breadcrumbs = new() { new ContentfulReferenceBuilder().Build() };
    private List<ContentfulAlert> _alerts = new() { new ContentfulAlertBuilder().Build() };
    private readonly List<Asset> _documents = new() { new ContentfulDocumentBuilder().Build() };
    private readonly List<ContentfulProfile> _profiles = new() { new ContentfulProfileBuilder().Build() };
    private List<ContentfulSection> _sections = new() { new ContentfulSectionBuilder().Build() };
    private string _systemId = "id";
    private string _contentTypeSystemId = "id";
    private readonly DateTime _updatedAt = DateTime.Now;
    private readonly DateTime _createdAt = DateTime.Now;
    public Asset Image { get => _image; set => _image = value; }
    private List<ContentfulGroupBranding> _articleBranding = new()
    {
        new ContentfulGroupBranding()
        {
            Title = "branding title",
            Text = "branding text",
            File = new Asset(),
            Url = "branding-url"
        }
    };

    private List<ContentfulReference> _relatedContent = new() { new ContentfulReferenceBuilder().Build() };

    public ContentfulArticle Build()
        => new()
        {
            Alerts = _alerts,
            BackgroundImage = _backgroundImage,
            Body = _body,
            Breadcrumbs = _breadcrumbs,
            Documents = _documents,
            Icon = _icon,
            Profiles = _profiles,
            Slug = _slug,
            Title = _title,
            Teaser = _teaser,
            Sections = _sections,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Image = Image,
            AlertsInline = _alertsInline,
            Sys = new SystemProperties
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                Id = _systemId,
                UpdatedAt = _updatedAt,
                CreatedAt = _createdAt,
            },
            ArticleBranding = _articleBranding,
            RelatedContent = _relatedContent
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

    public ContentfulArticleBuilder Section(ContentfulSection section)
    {
        _sections.Add(section);
        return this;
    }

    public ContentfulArticleBuilder WithOutSection()
    {
        _sections = new List<ContentfulSection>();
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

    public ContentfulArticleBuilder SystemContentTypeId(string id)
    {
        _contentTypeSystemId = id;
        return this;
    }

    public ContentfulArticleBuilder WithBreadcrumbContentType(string contentType)
    {
        if (_breadcrumbs.Any())
            _breadcrumbs[0].Sys.ContentType.SystemProperties.Id = contentType;

        return this;
    }
}