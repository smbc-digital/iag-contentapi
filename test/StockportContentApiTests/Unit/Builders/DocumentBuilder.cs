namespace StockportContentApiTests.Unit.Builders;

public class DocumentBuilder
{
    private readonly string _assetId = "asset id";
    private readonly string _title = "title";
    private readonly int _size = 22;
    private readonly string _url = "url";
    private readonly DateTime _lastUpdated = DateTime.MinValue;
    private readonly string _fileName = "fileName";
    private readonly string _mediaType = "mediatype";

    public Document Build() => new()
    {
        Title = _title,
        Size = _size,
        LastUpdated = _lastUpdated,
        Url = _url,
        FileName = _fileName,
        AssetId = _assetId,
        MediaType = _mediaType
    };
}