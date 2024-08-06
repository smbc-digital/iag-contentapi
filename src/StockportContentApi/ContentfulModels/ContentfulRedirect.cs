namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulRedirect
{
    public string Title { get; set; } = string.Empty;
    public Dictionary<string, string> Redirects { get; set; } = new();
    public Dictionary<string, string> LegacyUrls { get; set; } = new();
    public SystemProperties Sys { get; set; } = new();
}