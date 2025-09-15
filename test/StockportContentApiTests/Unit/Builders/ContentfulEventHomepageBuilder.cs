namespace StockportContentApiTests.Unit.Builders;

public class ContentfulEventHomepageBuilder
{
    private readonly List<ContentfulAlert> _alerts = new()
    {
        new ContentfulAlertBuilder().Build()
    };
    
    public ContentfulEventHomepage Build()
        => new()
        {
            TagOrCategory1 = "TagOrCategory1",
            TagOrCategory2 = "TagOrCategory2",
            TagOrCategory3 = "TagOrCategory3",
            TagOrCategory4 = "TagOrCategory4",
            TagOrCategory5 = "TagOrCategory5",
            TagOrCategory6 = "TagOrCategory6",
            TagOrCategory7 = "TagOrCategory7",
            TagOrCategory8 = "TagOrCategory8",
            TagOrCategory9 = "TagOrCategory9",
            TagOrCategory10 = "TagOrCategory10",
            MetaDescription = "meta description",
            Alerts = _alerts,
            CallToAction = new ContentfulCallToActionBannerBuilder().Build(),
            GlobalAlerts = _alerts,
            Sys = new()
        };
}