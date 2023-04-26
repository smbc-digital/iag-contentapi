namespace StockportContentApi.Utils;

public interface IUrlBuilder
{
    string UrlFor(string type, int referenceLevel = -1, bool displayOnAtoZ = false, string slug = null,
        int limit = -1, string tag = null);
}
public class UrlBuilder : IUrlBuilder
{
    private readonly string _contentfulApiUrl;


    public UrlBuilder(string contentfulApiUrl)
    {
        _contentfulApiUrl = contentfulApiUrl;
    }

    public string UrlFor(string type, int referenceLevel = -1, bool displayOnAtoZ = false, string slug = null, int limit = -1, string tag = null)
    {
        var baseUrl = $"{_contentfulApiUrl}&content_type={type}";
        if (displayOnAtoZ)
            baseUrl = $"{baseUrl}&fields.displayOnAZ=true";
        if (referenceLevel >= 0)
            baseUrl = $"{baseUrl}&include={referenceLevel}";
        if (!string.IsNullOrWhiteSpace(slug))
            baseUrl = $"{baseUrl}&fields.slug={slug}";
        if (!string.IsNullOrWhiteSpace(tag))
            baseUrl = string.Concat(baseUrl, $"&fields.tags[{GetSearchTypeForTag(ref tag)}]={WebUtility.UrlEncode(tag)}");
        if (limit >= 0)
            baseUrl = $"{baseUrl}&limit={limit}";

        return baseUrl;
    }

    private static string GetSearchTypeForTag(ref string tag)
    {
        if (string.IsNullOrEmpty(tag) || !tag.StartsWith("#")) return "in";
        tag = tag.Remove(0, 1);

        return "match";
    }
}
