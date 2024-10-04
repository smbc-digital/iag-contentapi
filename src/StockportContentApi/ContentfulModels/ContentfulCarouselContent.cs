namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulCarouselContent : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public DateTime SunriseDate { get; set; }
    public DateTime SunsetDate { get; set; }
    public string Url { get; set; }
    public SystemProperties Sys { get; set; } = new SystemProperties();
}
