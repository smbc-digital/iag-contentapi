namespace StockportContentApiTests.Unit.Builders;

public class ContentfulNewsBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private string _teaser = "teaser";
    private string _body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
    private DateTime _sunriseDate = new(2016, 06, 30, 0, 0, 0, DateTimeKind.Utc);
    private DateTime _sunsetDate = new(2017, 01, 30, 23, 0, 0, DateTimeKind.Utc);
    private List<string> _tags = new() { "Bramall Hall", "Events" };
    private readonly List<ContentfulAlert> _alerts = new() { new ContentfulAlertBuilder().Build() };

    private List<string> _categories = new() { "A category" };

    public ContentfulNews Build()
        => new()
        {
            Title = _title,
            Slug = _slug,
            Teaser = _teaser,
            Image = new ContentfulAssetBuilder().Url("image.jpg").Build(),
            Body = _body,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Tags = _tags,
            Alerts = _alerts,
            Categories = _categories,
            Sys = { UpdatedAt = new(2017, 01, 30, 23, 0, 0, DateTimeKind.Utc) }
        };

    public ContentfulNewsBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulNewsBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulNewsBuilder Teaser(string teaser)
    {
        _teaser = teaser;
        return this;
    }

    public ContentfulNewsBuilder SunriseDate(DateTime sunriseDate)
    {
        _sunriseDate = sunriseDate;
        return this;
    }

    public ContentfulNewsBuilder SunsetDate(DateTime sunsetDate)
    {
        _sunsetDate = sunsetDate;
        return this;
    }

    public ContentfulNewsBuilder Body(string body)
    {
        _body = body;
        return this;
    }

    public ContentfulNewsBuilder Tags(List<string> tags)
    {
        _tags = tags;
        return this;
    }

    public ContentfulNewsBuilder Categories(List<string> categories)
    {
        _categories = categories;
        return this;
    }
}