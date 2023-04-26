namespace StockportContentApi.ContentfulModels;

public class ContentfulContactUsCategory : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string BodyTextLeft { get; set; } = string.Empty;
    public string BodyTextRight { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new SystemProperties();
}