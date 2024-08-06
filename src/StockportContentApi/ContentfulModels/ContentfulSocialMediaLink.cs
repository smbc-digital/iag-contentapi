namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulSocialMediaLink : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string ScreenReader { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new();
}