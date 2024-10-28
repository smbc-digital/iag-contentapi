namespace StockportContentApiTests.Unit.Builders;

public class ContentfulEventBannerBuilder
{
    private string _title = "title";
    private string _teaser = "teaser";
    private string _icon = "icon";
    private string _link = "link";
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };

    public ContentfulEventBanner Build()
        => new()
        {
            Title = _title,
            Teaser = _teaser,
            Icon = _icon,
            Link = _link,
            Sys = _sys
        };

    public ContentfulEventBannerBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulEventBannerBuilder WithTeaser(string teaser)
    {
        _teaser = teaser;
        return this;
    }

    public ContentfulEventBannerBuilder WithIcon(string icon)
    {
        _icon = icon;
        return this;
    }

    public ContentfulEventBannerBuilder WithLink(string link)
    {
        _link = link;
        return this;
    }
}