namespace StockportContentApiTests.Unit.Builders;

class ContentfulCarouselContentBuilder
{
    private readonly string _title = "title";
    private readonly string _slug = "slug";
    private readonly string _teaser = "teaser";
    private readonly Asset _image = new() { File = new File { Url = "image.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
    private readonly DateTime _sunriseDate = new(2016, 9, 1, 0, 0, 0, DateTimeKind.Utc);
    private readonly DateTime _sunsetDate = new(2016, 9, 30, 0, 0, 0, DateTimeKind.Utc);
    private readonly string _url = "url";
    private readonly SystemProperties _sys = new() { Type = "Entry" };

    public ContentfulCarouselContent Build()
    {
        return new ContentfulCarouselContent()
        {
            Title = _title,
            Slug = _slug,
            Teaser = _teaser,
            Image = _image,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Url = _url,
            Sys = _sys
        };
    }


}
