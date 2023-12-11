namespace StockportContentApi.ContentfulModels;

public class ContentfulDirectory : ContentfulReference
{
    public string Body { get; set; } = string.Empty;
    public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    public ContentfulCallToActionBanner CallToAction { get; set; }
}
