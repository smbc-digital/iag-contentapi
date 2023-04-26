namespace StockportContentApi.ContentfulModels;

public class ContentfulSection : ContentfulReference
{
    public string Body { get; set; } = string.Empty;
    public List<ContentfulProfile> Profiles { get; set; } = new List<ContentfulProfile>();
    public List<Asset> Documents { get; set; } = new List<Asset>();
    public List<ContentfulAlert> AlertsInline { get; set; } = new List<ContentfulAlert>();
}
