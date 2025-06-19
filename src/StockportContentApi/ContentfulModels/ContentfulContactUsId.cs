namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulContactUsId : IContentfulModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new();
}