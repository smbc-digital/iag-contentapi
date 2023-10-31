namespace StockportContentApi.ContentfulModels;

public class ContentfulStartPage : ContentfulReference
{
    public string Summary { get; set; } = string.Empty;
    public string UpperBody { get; set; } = string.Empty;
    public string FormLinkLabel { get; set; } = string.Empty;
    public string FormLink { get; set; } = string.Empty;
    public string LowerBody { get; set; } = string.Empty;
    public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
    public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    public IEnumerable<ContentfulAlert> AlertsInline { get; set; } = new List<ContentfulAlert>();
}