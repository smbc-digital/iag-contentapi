namespace StockportContentApiTests.Unit.Builders;

public class ContentfulDocumentBuilder
{
    public Asset Build()
        => new ContentfulAssetBuilder()
            .Url("url.pdf")
            .FileName("fileName")
            .Description("documentTitle")
            .FileSize(674192)
            .UpdatedAt(new(2016, 10, 05, 00, 00, 00, DateTimeKind.Utc))
            .Title("title")
            .Build();
}