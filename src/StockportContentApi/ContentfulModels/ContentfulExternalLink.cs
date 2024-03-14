namespace StockportContentApi.ContentfulModels;

public class ContentfulExternalLink : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string URL { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new SystemProperties();
}
