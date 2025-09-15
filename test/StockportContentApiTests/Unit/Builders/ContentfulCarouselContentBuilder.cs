namespace StockportContentApiTests.Unit.Builders;

class ContentfulCarouselContentBuilder
{
    private readonly Asset _image = new() { File = new File { Url = "image.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
    private readonly SystemProperties _sys = new() { Type = "Entry" };

    public ContentfulCarouselContent Build()
        => new()
        {
            Title = "title",
            Slug = "slug",
            Teaser = "teaser",
            Image = _image,
            SunriseDate = new(2016, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            SunsetDate = new(2016, 9, 30, 0, 0, 0, DateTimeKind.Utc),
            Url = "url",
            Sys = _sys
        };
}