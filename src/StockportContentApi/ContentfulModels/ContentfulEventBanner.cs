namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulEventBanner : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public EColourScheme Colour { get; set; } = EColourScheme.Teal;
    public SystemProperties Sys { get; set; } = new();
    public ContentfulMetadata Metadata { get; set; } = new();
}