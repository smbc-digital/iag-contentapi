namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulSectionForSiteMap : IContentfulModel
{
    public string Slug { get; set; } = string.Empty;
    public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
    public SystemProperties Sys { get; set; } = new();
}