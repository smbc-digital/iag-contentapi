namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulAtoZ : IContentfulModel
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public string DisplayOnAZ { get; set; } = string.Empty;
    public List<string> AlternativeTitles { get; set; } = null;
    public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime SunsetDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public SystemProperties Sys { get; set; } = new SystemProperties();
}