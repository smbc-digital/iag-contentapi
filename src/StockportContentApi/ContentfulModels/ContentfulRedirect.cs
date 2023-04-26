namespace StockportContentApi.ContentfulModels;

public class ContentfulRedirect
{
    public string Title { get; set; } = string.Empty;
    public Dictionary<string, string> Redirects { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> LegacyUrls { get; set; } = new Dictionary<string, string>();
    public SystemProperties Sys { get; set; } = new SystemProperties();
}
