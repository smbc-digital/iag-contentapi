namespace StockportContentApiTests.Unit.Builders;

public class ContentfulSectionBuilder
{
    private readonly List<ContentfulAlert> _alertsInline = new()
    {
        new ContentfulAlertBuilder().Build()
    };

    private readonly string _body = "body";
    private readonly List<Asset> _documents = new() { new ContentfulDocumentBuilder().Build() };
    private readonly string _metaDescription = "metaDescription";

    private readonly List<ContentfulProfile> _profiles = new()
    {
        new ContentfulProfileBuilder().Build()
    };

    private readonly string _slug = "slug";
    private readonly DateTime _sunriseDate = DateTime.MinValue;
    private readonly DateTime _sunsetDate = DateTime.MinValue;
    private readonly string _title = "title";
    private readonly DateTime _updatedAt = DateTime.Now;

    public ContentfulSection Build() =>
        new()
        {
            Title = _title,
            Slug = _slug,
            MetaDescription = _metaDescription,
            Body = _body,
            Profiles = _profiles,
            Documents = _documents,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            UpdatedAt = _updatedAt,
            AlertsInline = _alertsInline,
            Sys = new()
            {
                ContentType = new() { SystemProperties = new() { Id = "id" } },
                UpdatedAt = _updatedAt
            }
        };
}