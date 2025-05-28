namespace StockportContentApiTests.Unit.Builders;

public class ContentfuTrustedLogoBuilder
{
    private string _title = "title";
    private string _text = "text";
    private Asset _image= new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
    private string _link = "url";

    public ContentfulTrustedLogo Build()
        => new()
        {
            Title = _title,
            Text = _text,
            Image = _image,
            Link = _link,
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

    public ContentfuTrustedLogoBuilder WithFile(Asset image)
    {
        _image = image;
        return this;
    }
    
    public ContentfuTrustedLogoBuilder WithUrl(string link)
    {
        _link = link;
        return this;
    }
}