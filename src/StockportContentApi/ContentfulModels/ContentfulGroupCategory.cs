namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulGroupCategory : IContentfulModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public Asset Image { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public SystemProperties Sys { get; set; } = new();
}