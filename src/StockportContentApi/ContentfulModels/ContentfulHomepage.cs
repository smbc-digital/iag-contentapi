namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulHomepage : IContentfulModel
{
    public string FeaturedTasksHeading { get; set; } = string.Empty;
    public string FeaturedTasksSummary { get; set; } = string.Empty;
    public IEnumerable<ContentfulReference> FeaturedTasks { get; set; } = new List<ContentfulReference>();
    public IEnumerable<ContentfulReference> FeaturedTopics { get; set; } = new List<ContentfulReference>();
    public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    public IEnumerable<ContentfulCarouselContent> CarouselContents { get; set; } = new List<ContentfulCarouselContent>();
    public Asset BackgroundImage { get; set; } = new();
    public Asset ForegroundImage { get; set; } = new();
    public string ForegroundImageLocation { get; set; } = string.Empty;
    public string ForegroundImageLink { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string FreeText { get; set; } = string.Empty;
    public string EventCategory { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new();
    public string MetaDescription { get; set; } = string.Empty;
    public ContentfulCarouselContent CampaignBanner { get; set; } = new();
    public ContentfulCallToActionBanner CallToAction { get; set; } = null;
    public ContentfulCallToActionBanner CallToActionPrimary { get; set; } = null;
    public IEnumerable<ContentfulSpotlightOnBanner> SpotlightOnBanner { get; set; } = null;
    public string ImageOverlayText { get; set; } = string.Empty;
    public ContentfulMetadata Metadata { get; set; } = new();
}