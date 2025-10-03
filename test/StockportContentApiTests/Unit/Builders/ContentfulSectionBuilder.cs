namespace StockportContentApiTests.Unit.Builders;

public class ContentfulSectionBuilder
{
    private readonly List<ContentfulAlert> _alertsInline = new()
    {
        new ContentfulAlertBuilder().Build()
    };

    private readonly List<Asset> _documents = new() { new ContentfulDocumentBuilder().Build() };

    private readonly List<ContentfulProfile> _profiles = new()
    {
        new ContentfulProfileBuilder().Build()
    };

    public ContentfulSection Build()
        => new()
        {
            Title = "title",
            Slug = "slug",
            MetaDescription = "metaDescription",
            Body = "body",
            Profiles = _profiles,
            Documents = _documents,
            SunriseDate = DateTime.MinValue,
            SunsetDate = DateTime.MinValue,
            UpdatedAt = DateTime.Now,
            AlertsInline = _alertsInline,
            Sys = new()
            {
                ContentType = new() { SystemProperties = new() { Id = "id" } },
                UpdatedAt = DateTime.Now
            }
        };
}