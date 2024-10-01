namespace StockportContentApiTests.Unit.Builders;

public class ContentfulEventBannerBuilder
{
    private readonly string _title = "title";
    private readonly string _teaser = "teaser";
    private readonly string _icon = "icon";
    private readonly string _link = "link";
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };

    public ContentfulEventBanner Build()
    {
        return new ContentfulEventBanner
        {
            Title = _title,
            Teaser = _teaser,
            Icon = _icon,
            Link = _link,
            Sys = _sys
        };
    }

}
