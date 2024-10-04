namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Document
{
    public string Title { get; set; }
    public int Size { get; set; }
    public string Url { get; set; }
    public DateTime LastUpdated { get; set; }
    public string FileName { get; set; }
    public string AssetId { get; set; }
    public string MediaType { get; set; }
}