namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulTrivia : IContentfulModel
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Statistic { get; set; } = string.Empty;
    public string StatisticSubHeading { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; }
}