namespace StockportContentApiTests.Unit.Builders;

public class ContentfulHomepageBuilder
{
    private readonly string _featuredTasksHeading = "Featured tasks heading";
    private readonly string _featuredTasksSummary = "Featured tasks summary";
    private readonly Asset _backgroundImage = new() { File = new File { Url = "image.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
    private readonly string _freeText = "homepage text";
    private readonly string _metaDescription = "meta description";
   
    private readonly List<ContentfulCarouselContent> _carouselContents = new()
    {
        new ContentfulCarouselContentBuilder().Build()
    };

    private readonly List<ContentfulAlert> _alerts = new()
    {
        new ContentfulAlertBuilder().Build()
    };

    private readonly List<ContentfulReference> _featuredTasks = new();

    private readonly List<ContentfulReference> _featuredTopics = new()
    {
        new ContentfulTopicBuilder().Build()
    };

    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };

    private readonly ContentfulCarouselContent _campaignBanner = new ContentfulCarouselContentBuilder().Build();

    public ContentfulHomepage Build()
        => new()
        {
            FeaturedTasksHeading = _featuredTasksHeading,
            FeaturedTasksSummary = _featuredTasksSummary,
            FeaturedTasks = _featuredTasks,
            FeaturedTopics = _featuredTopics,
            Alerts = _alerts,
            CarouselContents = _carouselContents,
            BackgroundImage = _backgroundImage,
            FreeText = _freeText,
            Sys = _sys,
            MetaDescription = _metaDescription,
            CampaignBanner = _campaignBanner
        };
}