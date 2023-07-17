namespace StockportContentApi.ContentfulModels;

public class ContentfulHomepage : IContentfulModel
{
    public IEnumerable<string> PopularSearchTerms { get; set; } = new List<string>();
    public string FeaturedTasksHeading { get; set; } = string.Empty;
    public string FeaturedTasksSummary { get; set; } = string.Empty;
    public IEnumerable<ContentfulReference> FeaturedTasks { get; set; } = new List<ContentfulReference>();
    public IEnumerable<ContentfulReference> FeaturedTopics { get; set; } = new List<ContentfulReference>();
    public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    public IEnumerable<ContentfulCarouselContent> CarouselContents { get; set; } = new List<ContentfulCarouselContent>();
    public Asset BackgroundImage { get; set; } = new Asset();
    public string FreeText { get; set; } = string.Empty;
    public IEnumerable<ContentfulGroup> FeaturedGroups { get; set; } = new List<ContentfulGroup>();
    public string EventCategory { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new SystemProperties();
    public string MetaDescription { get; set; } = string.Empty;
    public ContentfulCarouselContent CampaignBanner { get; set; } = new ContentfulCarouselContent();
    public ContentfulCallToActionBanner CallToAction { get; set; } = null;
}
