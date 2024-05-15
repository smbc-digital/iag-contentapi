namespace StockportContentApi.ContentfulModels;

public class ContentfulArticleForSiteMap : IContentfulModel
{
    public string Slug { get; set; } = string.Empty;
    public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
    public List<ContentfulSectionForSiteMap> Sections { get; set; } = new();
    public SystemProperties Sys { get; set; } = new();
}
