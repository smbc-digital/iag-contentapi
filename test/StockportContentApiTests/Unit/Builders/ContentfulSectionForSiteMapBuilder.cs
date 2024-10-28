namespace StockportContentApiTests.Unit.Builders;

public class ContentfulSectionForSiteMapBuilder
{
    private readonly string _slug = "slug";
    private readonly DateTime _sunriseDate = DateTime.MinValue;
    private readonly DateTime _sunsetDate = DateTime.MinValue;
    private readonly DateTime _updatedAt = DateTime.Now;

    public ContentfulSectionForSiteMap Build()
        => new()
        {
            Slug = _slug,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Sys = new()
            {
                ContentType = new() { SystemProperties = new() { Id = "id" } },
                UpdatedAt = _updatedAt
            }
        };
}