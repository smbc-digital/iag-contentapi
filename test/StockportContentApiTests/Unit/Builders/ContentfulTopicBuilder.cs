namespace StockportContentApiTests.Unit.Builders;

public class ContentfulTopicBuilder
{
    private string _slug = "slug";
    private readonly Asset _backgroundImage = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
    private readonly Asset _image = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
    private readonly List<ContentfulReference> _breadcrumbs = new() { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build() };
    private List<ContentfulAlert> _alerts = new(){ new ContentfulAlertBuilder().Build() };
    private readonly List<ContentfulReference> _subItems = new() { new ContentfulReferenceBuilder().Slug("sub-slug").Build() };
    private readonly List<ContentfulReference> _secondaryItems = new() { new ContentfulReferenceBuilder().Slug("secondary-slug").Build() };
    private readonly ContentfulCallToActionBanner _callToActionBanner = new ContentfulCallToActionBannerBuilder().Build();
    private readonly ContentfulEventBanner _eventBanner = new ContentfulEventBannerBuilder().Build();

    public ContentfulTopic Build() => new()
        {
            Slug = _slug,
            Title = "name",
            Teaser = "teaser",
            MetaDescription = "metaDescription",
            Summary = "summary",
            Icon = "icon",
            BackgroundImage = _backgroundImage,
            Image = _image,
            SubItems = _subItems,
            SecondaryItems = _secondaryItems,
            CallToAction = _callToActionBanner,
            Breadcrumbs = _breadcrumbs,
            Alerts = _alerts,
            SunriseDate = DateTime.MinValue,
            SunsetDate = DateTime.MaxValue,
            EventBanner = _eventBanner,
            DisplayContactUs = false,
            Sys = new SystemProperties()
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } },
                Id = "id"
            }
        };

    public ContentfulTopicBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulTopicBuilder Alerts(List<ContentfulAlert> alerts)
    {
        _alerts = alerts;
        return this;
    }
}