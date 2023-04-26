namespace StockportContentApi.ContentfulModels;

public class ContentfulExpandingLinkBox : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public List<ContentfulReference> Links { get; set; } = new List<ContentfulReference>();
    public SystemProperties Sys { get; set; } = new SystemProperties();
}
