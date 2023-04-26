namespace StockportContentApi.Model;

public class Document
{
    public string Title { get; set; }
    public int Size { get; set; }
    public string Url { get; set; }
    public DateTime LastUpdated { get; set; }
    public string FileName { get; set; }
    public string AssetId { get; set; }
    public string MediaType { get; set; }

    public Document(string title, int size, DateTime lastUpdated, string url, string fileName, string assetId, string mediaType)
    {
        Title = title;
        Size = size;
        Url = url;
        LastUpdated = lastUpdated;
        FileName = fileName;
        AssetId = assetId;
        MediaType = mediaType;
    }
}
