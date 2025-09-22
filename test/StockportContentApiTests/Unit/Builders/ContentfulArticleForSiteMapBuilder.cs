namespace StockportContentApiTests.Unit.Builders;

public class ContentfulArticleForSiteMapBuilder
{
    private DateTime _sunriseDate = new(2016, 1, 10, 0, 0, 0, DateTimeKind.Utc);
    private DateTime _sunsetDate = new(2017, 1, 20, 0, 0, 0, DateTimeKind.Utc);

    public ContentfulArticleForSiteMap Build()
        => new()
        {
            Slug = "slug",
            Sections = new() { new ContentfulSectionForSiteMapBuilder().Build() },
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Sys = new SystemProperties
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } },
                Id = "id",
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            }
        };

    public ContentfulArticleForSiteMapBuilder WithSunrise(DateTime sunrise)
    {
        _sunriseDate = sunrise;
        return this;
    }

    public ContentfulArticleForSiteMapBuilder WithSunset(DateTime sunset)
    {
        _sunsetDate = sunset;
        return this;
    }
}