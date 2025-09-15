namespace StockportContentApiTests.Unit.Builders;
public class DirectoryEntryBuilder
{
    private string _slug = "slug";
    private readonly MapPosition _mapPosition = new()
    {
        Lat = 53.393310,
        Lon = -2.126633
    };

    private readonly List<ContentfulAlert> _alerts = new() { new ContentfulAlertBuilder().WithSeverity("Warning").Build() };
    private readonly List<ContentfulAlert> _alertsInline = new()
    {
        new ContentfulAlertBuilder().WithSeverity("Condolence").Build(),
        new ContentfulAlertBuilder().WithSeverity("Information").Build()
    };
    private readonly List<ContentfulFilter> _filters = new();
    private readonly List<ContentfulTrustedLogo> _trustedLogos = new()
    {
        new() {
            Image = new Asset(),
            Sys = new SystemProperties(),
            Text = "test",
            Title = "test",
            Link = "test"
        }
    };

    private readonly List<ContentfulDirectory> _directories = new()
    {
        new DirectoryBuilder()
        .WithSlug("test-alert")
        .WithTitle("Test Alert")
        .WithBody("Test Alert Body")
        .Build()
    };

    public ContentfulDirectoryEntry Build()
        => new()
        {
            Slug = _slug,
            Title = "title",
            Provider = "provider",
            Description = "description",
            MetaDescription = "meta description",
            Teaser = "teaser",
            PhoneNumber = "0123456789",
            Email = "email",
            Website = "website",
            Twitter = "twitter",
            Facebook = "facebook",
            Address = "address",
            Filters = _filters,
            Alerts = _alerts,
            AlertsInline = _alertsInline,
            MapPosition = _mapPosition,
            TrustedLogos = _trustedLogos,
            Directories = _directories,
        };

    public DirectoryEntryBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public DirectoryEntryBuilder WithFilter(string slug, string title, string displayName, string theme)
    {
        _filters.Add(new ContentfulFilter
        {
            Slug = slug,
            Title = title,
            DisplayName = displayName,
            Theme = theme
        });

        return this;
    }
}