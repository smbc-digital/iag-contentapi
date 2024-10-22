namespace StockportContentApiTests.Unit.Builders;

public class ContentfulExternalLinkBuilder
{
    private string _title = "title";
    private string _url = "url";
    private string _teaser = "teaser";

    public ContentfulExternalLink Build()
        => new()
        {
            Title = _title,
            URL = _url,
            Teaser = _teaser,
        };
    
    public ContentfulExternalLinkBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulExternalLinkBuilder WithURL(string url)
    {
        _url = url;
        return this;
    }

    public ContentfulExternalLinkBuilder WithTeaser(string teaser)
    {
        _teaser = teaser;
        return this;
    }
}