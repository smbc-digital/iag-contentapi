namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulNewsRoom : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public bool EmailAlerts { get; set; } = false;
    public string EmailAlertsTopicId { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new();
}