namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class BusinessIdToRedirects(IDictionary<string, string> shortUrlRedirects, IDictionary<string, string> legacyUrlRedirects)
{
    public readonly RedirectDictionary ShortUrlRedirects = new(shortUrlRedirects);
    public readonly RedirectDictionary LegacyUrlRedirects = new(legacyUrlRedirects);
}

public class NullBusinessIdToRedirects : BusinessIdToRedirects
{
    public NullBusinessIdToRedirects() : base(new RedirectDictionary(), new RedirectDictionary()) { }
}