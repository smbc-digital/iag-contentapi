namespace StockportContentApi.ContentfulModels;

public class ContentfulArticle : ContentfulReference
{
    public string Body { get; set; } = string.Empty;
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public List<ContentfulProfile> Profiles { get; set; } = new();
    public List<Asset> Documents { get; set; } = new();
    public List<ContentfulAlert> AlertsInline { get; set; } = new();
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
}