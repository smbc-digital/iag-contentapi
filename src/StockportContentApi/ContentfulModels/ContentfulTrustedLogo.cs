namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulTrustedLogo : IContentfulModel
{
    public string Title { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public Asset Image { get; set; } = new();

    public string Link { get; set; } = string.Empty;

    public SystemProperties Sys { get; set; } = new();
    public ContentfulMetadata Metadata { get; set; } = new();
}