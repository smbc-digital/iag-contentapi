namespace StockportContentApiTests.Unit.Builders;

public class ContentfulDocumentBuilder
{
    private string _description = "documentTitle";
    private string _title = "title";
    private string _url = "url.pdf";
    private int _size = 674192;
    private string _fileName = "fileName";
    private DateTime _updatedAt = new DateTime(2016, 10, 05, 00, 00, 00, DateTimeKind.Utc);

    public Asset Build()
    {
        return new ContentfulAssetBuilder().Url(_url)
                                           .FileName(_fileName)
                                           .Description(_description)
                                           .FileSize(_size)
                                           .UpdatedAt(_updatedAt)
                                           .Title(_title)
                                           .Build();
    }
}
