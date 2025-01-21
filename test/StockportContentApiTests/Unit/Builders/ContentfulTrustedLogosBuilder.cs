namespace StockportContentApiTests.Unit.Builders;

public class ContentfulTrustedLogosBuilder
{
    private string _title = "title";
    private string _text = "text";
    private Asset _file = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
    private string _url = "url";

    public ContentfulTrustedLogos Build()
        => new()
        {
            Title = _title,
            Text = _text,
            File = _file,
            Url = _url,
        };

    public ContentfulTrustedLogosBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulTrustedLogosBuilder WithText(string text)
    {
        _text = text;
        return this;
    }

    public ContentfulTrustedLogosBuilder WithFile(Asset file)
    {
        _file = file;
        return this;
    }
    
    public ContentfulTrustedLogosBuilder WithUrl(string url)
    {
        _url = url;
        return this;
    }
}