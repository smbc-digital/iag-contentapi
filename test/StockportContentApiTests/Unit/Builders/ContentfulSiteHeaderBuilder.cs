namespace StockportContentApiTests.Unit.Builders;

public class ContentfulSiteHeaderBuilder
{
    private readonly string _title = "Header";
    private readonly Asset _logo = new()
    {
        File = new File
        {
            Url = "FileUrl"
        },
        SystemProperties = new SystemProperties
        {
            Type = "Asset"
        }
    };

    private readonly List<ContentfulReference> _items = new()
    {
      new ContentfulReferenceBuilder().Build()
    };

    public ContentfulSiteHeader Build()
    {
        return new ContentfulSiteHeader
        {
            Title = _title,
            Logo = _logo,
            Items = _items
        };
    }
}
