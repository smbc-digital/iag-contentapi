namespace StockportContentApiTests.Unit.Builders;

public class ContentfulSectionForSiteMapBuilder
{
    public ContentfulSectionForSiteMap Build()
        => new()
        {
            Slug = "slug",
            SunriseDate = DateTime.MinValue,
            SunsetDate = DateTime.MinValue,
            Sys = new()
            {
                ContentType = new() { SystemProperties = new() { Id = "id" } },
                UpdatedAt = DateTime.Now
            }
        };
}