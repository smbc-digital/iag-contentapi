namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulNewsRoom : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public bool EmailAlerts { get; set; } = false;
    public string EmailAlertsTopicId { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new();
    public ContentfulCallToActionBanner CallToAction { get; set; } = null;
    public ContentfulNews FeaturedNews { get; set; } = null;
    public ContentfulMetadata Metadata { get; set; } = new();
}