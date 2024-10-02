namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulSection : ContentfulReference
{
    public string Body { get; set; } = string.Empty;
    public List<ContentfulProfile> Profiles { get; set; } = new();
    public List<Asset> Documents { get; set; } = new();
    public List<ContentfulAlert> AlertsInline { get; set; } = new();
    public List<ContentfulGroupBranding> SectionBranding { get; set; } = new();
    public string LogoAreaTitle { get; set; }
    public DateTime UpdatedAt { get; set; }
}