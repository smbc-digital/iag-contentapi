namespace StockportContentApiTests.Unit.Builders;

public class ContentfulRedirectBuilder
{
    private string _title = "_title";
    private Dictionary<string, string> _redirects = new() { { "a-url", "another-url" } };
    private Dictionary<string, string> _legacyUrls = new() { { "some-url", "another-url" } };

    public ContentfulRedirect Build()
        => new()
        {
            Title = _title,
            LegacyUrls = _legacyUrls,
            Redirects = _redirects
        };

    public ContentfulRedirectBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulRedirectBuilder WithRedirects(Dictionary<string, string> redirects)
    {
        _redirects = redirects;
        return this;
    }

    public ContentfulRedirectBuilder WithLegacyUrls(Dictionary<string, string> legacyUrl)
    {
        _legacyUrls = legacyUrl;
        return this;
    }
}