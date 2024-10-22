namespace StockportContentApiTests.Unit.Builders;

public class ContentfulGroupBrandingBuilder
{
    private string _title = "title";
    private string _text = "text";
    private Asset _file = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
    private string _url = "url";

    public ContentfulGroupBranding Build()
        => new()
        {
            Title = _title,
            Text = _text,
            File = _file,
            Url = _url,
        };

    public ContentfulGroupBrandingBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulGroupBrandingBuilder WithText(string text)
    {
        _text = text;
        return this;
    }

    public ContentfulGroupBrandingBuilder WithFile(Asset file)
    {
        _file = file;
        return this;
    }
    
    public ContentfulGroupBrandingBuilder WithUrl(string url)
    {
        _url = url;
        return this;
    }
}