namespace StockportContentApiTests.Unit.Builders;

public class DocumentBuilder
{
    public Document Build()
        => new()
        {
            Title = "asset id",
            Size = 22,
            LastUpdated = DateTime.MinValue,
            Url = "url",
            FileName = "fileName",
            AssetId = "asset id",
            MediaType = "mediatype"
        };
}