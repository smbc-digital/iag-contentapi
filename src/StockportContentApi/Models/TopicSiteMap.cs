namespace StockportContentApi.Model;
[ExcludeFromCodeCoverage]
public class TopicSiteMap
{
    public string Slug { get; }
    public DateTime SunriseDate { get; }
    public DateTime SunsetDate { get; }

    public TopicSiteMap(string slug, DateTime sunriseDate, DateTime sunsetDate)
    {
        Slug = slug;
        SunriseDate = sunriseDate;
        SunsetDate = sunsetDate;
    }
}