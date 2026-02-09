namespace StockportContentApiTests.Unit.Builders;

public class ContentfulPublicationTemplateBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private Asset _headerImage = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
    private List<ContentfulReference> _breadcrumbs = new() { new ContentfulReferenceBuilder().Build() };
    private string _systemId = "id";
    private List<ContentfulPublicationPage> _publicationPages = new()
    {
        new ContentfulPublicationPageBuilder().Build()
    };

    public Asset HeaderImage
    {
        get => _headerImage;
        set => _headerImage = value;
    }

    private readonly List<ContentfulTrustedLogo> _trustedLogos = new()
    {
        new ContentfulTrustedLogo()
        {
            Title = "branding title",
            Text = "branding text",
            Image = new Asset(),
            Link = "branding-url"
        }
    };

    public ContentfulPublicationTemplate Build()
        => new()
        {
            Breadcrumbs = _breadcrumbs,
            Slug = _slug,
            Title = _title,
            Subtitle = "subtitle",
            Summary = "summary",
            MetaDescription = "meta description",
            HeaderImage = _headerImage,
            Sys = new SystemProperties
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } },
                Id = _systemId,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            },
            TrustedLogos = _trustedLogos,
            PublicationPages = _publicationPages
        };

    public ContentfulPublicationTemplateBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulPublicationTemplateBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulPublicationTemplateBuilder Breadcrumbs(List<ContentfulReference> breadcrumbs)
    {
        _breadcrumbs = breadcrumbs;
        return this;
    }

    public ContentfulPublicationTemplateBuilder SystemId(string id)
    {
        _systemId = id;
        return this;
    }

    public ContentfulPublicationTemplateBuilder WithBreadcrumbContentType(string contentType)
    {
        if (_breadcrumbs.Any())
            _breadcrumbs[0].Sys.ContentType.SystemProperties.Id = contentType;

        return this;
    }
}