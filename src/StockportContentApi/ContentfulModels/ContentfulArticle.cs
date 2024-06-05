namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulArticle : ContentfulReference
{
    public string Body { get; set; } = string.Empty;
    public string AltText { get; set; }
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public List<ContentfulProfile> Profiles { get; set; } = new();
    public List<Asset> Documents { get; set; } = new();
    public List<ContentfulAlert> AlertsInline { get; set; } = new();
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
    public List<ContentfulReference> RelatedContent { get; set; } = new();
    public List<ContentfulGroupBranding> ArticleBranding { get; set; } = new();
    public string LogoAreaTitle { get; set;}
}