namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulTopic : ContentfulReference
{
    public string Summary { get; set; } = string.Empty;
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
    public string VideoTeaser { get; init; }
    public string VideoTag { get; init; }
    public string TriviaSubheading { get; set; } = string.Empty;
    public List<ContentfulTrivia> TriviaSection { get; init; }
    public IEnumerable<ContentfulReference> FeaturedTasks { get; set; } = new List<ContentfulReference>();
    public ContentfulCallToActionBanner CallToAction { get; init; }
    public bool EmailAlerts { get; set; } = false;
    public string EmailAlertsTopicId { get; set; } = string.Empty;
    public ContentfulEventBanner EventBanner { get; set; } = new()
    {
        Sys = new() { Type = "Entry" }
    };
    public bool DisplayContactUs { get; set; } = true;
    public string EventCategory { get; set; }
    public ContentfulCarouselContent CampaignBanner { get; set; } = new();
    public List<ContentfulTrustedLogo> TrustedLogos { get; set; } = new();
    public string LogoAreaTitle { get; set; }
}