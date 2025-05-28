namespace StockportContentApiTests.Unit.Builders;

public class ContentfuTrustedLogoBuilder
{
    private string _title = "title";
    private string _text = "text";
    private Asset _file = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
    private string _url = "url";

    public ContentfulTrustedLogo Build()
        => new()
        {
            Title = _title,
            Text = _text,
            File = _file,
            Url = _url,
        };

    public ContentfuTrustedLogoBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfuTrustedLogoBuilder WithText(string text)
    {
        _text = text;
        return this;
    }

    public ContentfuTrustedLogoBuilder WithFile(Asset file)
    {
        _file = file;
        return this;
    }
    
    public ContentfuTrustedLogoBuilder WithUrl(string url)
    {
        _url = url;
        return this;
    }
}