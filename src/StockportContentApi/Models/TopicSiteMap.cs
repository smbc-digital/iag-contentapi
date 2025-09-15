namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class TopicSiteMap(string slug, DateTime sunriseDate, DateTime sunsetDate)
{
    public string Slug { get; } = slug;
    public DateTime SunriseDate { get; } = sunriseDate;
    public DateTime SunsetDate { get; } = sunsetDate;
}