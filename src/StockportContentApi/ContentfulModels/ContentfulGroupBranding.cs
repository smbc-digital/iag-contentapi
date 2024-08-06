namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulGroupBranding : IContentfulModel
{
    public string Title { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public Asset File { get; set; } = new();

    public string Url { get; set; } = string.Empty;

    public SystemProperties Sys { get; set; } = new();
}