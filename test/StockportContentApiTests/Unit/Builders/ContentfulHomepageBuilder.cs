namespace StockportContentApiTests.Unit.Builders;

public class ContentfulHomepageBuilder
{
    private readonly Asset _backgroundImage = new() { File = new File { Url = "image.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
   
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
            FeaturedTasksHeading =  "Featured tasks heading",
            FeaturedTasksSummary = "Featured tasks summary",
            FeaturedTasks = _featuredTasks,
            FeaturedTopics = _featuredTopics,
            Alerts = _alerts,
            CarouselContents = _carouselContents,
            BackgroundImage = _backgroundImage,
            FreeText = "homepage text",
            Sys = _sys,
            MetaDescription = "meta description",
            CampaignBanner = _campaignBanner
        };
}