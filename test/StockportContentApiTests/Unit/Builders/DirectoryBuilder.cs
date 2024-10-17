namespace StockportContentApiTests.Unit.Builders;
public class DirectoryBuilder
{
    private string _slug = "slug";
    private string _title = "title";
    private string _body = "body";
    private readonly string _teaser = "teaser";
    private readonly string _metaDescription = "meta description";
    private readonly string _id = "XXX123456";
    private readonly string _backgroundImageUrl = "//TESTIMAGE.JPG";
    private readonly List<ContentfulAlert> _alerts = new() { new ContentfulAlertBuilder().Build() };
    private readonly List<ContentfulAlert> _alertsInline = new()
    {
        new ContentfulAlertBuilder().WithSeverity("Condolence").Build(),
        new ContentfulAlertBuilder().WithSeverity("Warning").Build()
    };
    private readonly ContentfulCallToActionBanner _callToActionBanner = new ContentfulCallToActionBannerBuilder().Build();
    private readonly List<ContentfulReference> _relatedContent = new() { new ContentfulReferenceBuilder().Slug("sub-slug").Build() };
    private readonly List<ContentfulExternalLink> _externalLinks = new() { new ContentfulExternalLink() };
    private readonly List<ContentfulDirectoryEntry> _pinnedEntries = new() { new ContentfulDirectoryEntry() };
    private readonly List<ContentfulReference> _subItems = new() { new ContentfulReference() };
    private readonly List<ContentfulDirectory> _subDirectories = new() { new ContentfulDirectory() };

    public ContentfulDirectory Build()
        => new()
        {
            Slug = _slug,
            Title = _title,
            Body = _body,
            MetaDescription = _metaDescription,
            Teaser = _teaser,
            Sys = new SystemProperties()
            {
                Id = _id
            },
            BackgroundImage = new Asset()
            {
                File = new File
                {
                    Url = _backgroundImageUrl
                },
                SystemProperties = new SystemProperties()
                {
                    Type = "Image"
                }
            },
            CallToAction = _callToActionBanner,
            Alerts = _alerts,
            AlertsInline = _alertsInline,
            RelatedContent = _relatedContent,
            ExternalLinks = _externalLinks,
            PinnedEntries = _pinnedEntries,
            SubItems = _subItems,
            SubDirectories = _subDirectories
        };

    public DirectoryBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public DirectoryBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public DirectoryBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }
}