namespace StockportContentApiTests.Unit.Builders;

public class ContentfulPublicationPageBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private string _metaDescription = "meta description";
    private Contentful.Core.Models.Document _body = new();
    private List<ContentfulAlert> _inlineAlerts = new() { new ContentfulAlertBuilder().Build() };
    private List<ContentfulInlineQuote> _inlineQuotes = new() { new ContentfulInlineQuoteBuilder().Build() };
    private string _systemId = "id";
    private List<ContentfulPublicationSection> _publicationSections = new()
    {
        new ContentfulPublicationSectionBuilder().Build()
    };

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

    public ContentfulPublicationPage Build()
        => new()
        {
            BackgroundImage = new ContentfulAssetBuilder().Url("image-url.jpg").Build(),
            Body = _body,
            Slug = _slug,
            Title = _title,
            MetaDescription = _metaDescription,
            InlineAlerts = _inlineAlerts,
            InlineQuotes = _inlineQuotes,
            PublicationSections = _publicationSections,
            Sys = new SystemProperties
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } },
                Id = _systemId,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            },
            TrustedLogos = _articleBranding,
        };

    public ContentfulPublicationPageBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulPublicationPageBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulPublicationPageBuilder InlineAlerts(List<ContentfulAlert> inlineAlerts)
    {
        _inlineAlerts = inlineAlerts;
        return this;
    }

    public ContentfulPublicationPageBuilder InlineQuotes(List<ContentfulInlineQuote> inlineQuotes)
    {
        _inlineQuotes = inlineQuotes;
        return this;
    }

    public ContentfulPublicationPageBuilder Body(Contentful.Core.Models.Document body)
    {
        _body = body;
        return this;
    }

    public ContentfulPublicationPageBuilder SystemId(string id)
    {
        _systemId = id;
        return this;
    }
}