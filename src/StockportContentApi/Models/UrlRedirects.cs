namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class RedirectDictionary : Dictionary<string, string>
{
    public RedirectDictionary() { }
    public RedirectDictionary(IDictionary<string, string> dict) : base(dict) { }
}