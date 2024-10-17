namespace StockportContentApiTests.Unit.Builders;

public class ContentfulDocumentBuilder
{
    private readonly string _description = "documentTitle";
    private readonly string _title = "title";
    private readonly string _url = "url.pdf";
    private readonly int _size = 674192;
    private readonly string _fileName = "fileName";
    private readonly DateTime _updatedAt = new(2016, 10, 05, 00, 00, 00, DateTimeKind.Utc);

    public Asset Build()
        => new ContentfulAssetBuilder()
            .Url(_url)
            .FileName(_fileName)
            .Description(_description)
            .FileSize(_size)
            .UpdatedAt(_updatedAt)
            .Title(_title)
            .Build();
}