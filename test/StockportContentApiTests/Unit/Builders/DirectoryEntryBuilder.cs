namespace StockportContentApiTests.Unit.Builders;
public class DirectoryEntryBuilder
{
    private string _slug = "slug";
    private readonly string _title = "title";
    private readonly string _provider = "provider";
    private readonly string _description = "description";
    private readonly string _teaser = "teaser";
    private readonly string _metaDescription = "meta description";
    private readonly string _phoneNumber = "0123456789";
    private readonly string _email = "email";
    private readonly string _website = "website";
    private readonly string _twitter = "twitter";
    private readonly string _facebook = "facebook";
    private readonly string _address = "address";
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
    private readonly List<ContentfulGroupBranding> _branding = new()
    {
        new() {
            File = new Asset(),
            Sys = new SystemProperties(),
            Text = "test",
            Title = "test",
            Url = "test"
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
            Title = _title,
            Provider = _provider,
            Description = _description,
            MetaDescription = _metaDescription,
            Teaser = _teaser,
            PhoneNumber = _phoneNumber,
            Email = _email,
            Website = _website,
            Twitter = _twitter,
            Facebook = _facebook,
            Address = _address,
            Filters = _filters,
            Alerts = _alerts,
            AlertsInline = _alertsInline,
            MapPosition = _mapPosition,
            GroupBranding = _branding,
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