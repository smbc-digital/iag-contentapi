namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulEventBanner : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new();
}