namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulSiteHeader
{
    public string Title { get; set; } = string.Empty;
        
    public List<ContentfulReference> Items { get; set; } = new();

    public Asset Logo { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
}