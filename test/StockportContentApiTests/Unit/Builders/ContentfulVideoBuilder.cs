namespace StockportContentApiTests.Unit.Builders;

public class ContentfulVideoBuilder
{
    private string _heading = "heading";
    private string _text = "text";
    private string _videoEmbedCode = "video embed code";

    public ContentfulVideo Build()
        => new()
        {
            Heading = _heading,
            Text = _text,
            VideoEmbedCode = _videoEmbedCode
        };

    public ContentfulVideoBuilder WithHeading(string heading)
    {
        _heading = heading;
        return this;
    }

    public ContentfulVideoBuilder WithText(string text)
    {
        _text = text;
        return this;
    }

    public ContentfulVideoBuilder WithVideoEmbedCode(string videoEmbedCode)
    {
        _videoEmbedCode = videoEmbedCode;
        return this;
    }
}