namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Redirects(Dictionary<string, RedirectDictionary> shortUrlRedirects, Dictionary<string, RedirectDictionary> legacyUrlRedirects)
{
    public Dictionary<string, RedirectDictionary> ShortUrlRedirects = shortUrlRedirects;
    public Dictionary<string, RedirectDictionary> LegacyUrlRedirects = legacyUrlRedirects;
}

[ExcludeFromCodeCoverage]

public class ShortUrlRedirects(Dictionary<string, RedirectDictionary> redirects)
{
    public Dictionary<string, RedirectDictionary> Redirects = redirects;
}

[ExcludeFromCodeCoverage]
public class LegacyUrlRedirects(Dictionary<string, RedirectDictionary> redirects)
{
    public Dictionary<string, RedirectDictionary> Redirects = redirects;
}