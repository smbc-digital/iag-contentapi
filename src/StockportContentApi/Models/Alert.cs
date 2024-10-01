namespace StockportContentApi.Models;
[ExcludeFromCodeCoverage]
public class Alert
{
    public string Title { get; }
    public string SubHeading { get; }
    public string Body { get; }
    public string Severity { get; }
    public DateTime SunriseDate { get; }
    public DateTime SunsetDate { get; }
    public string Slug { get; set; }
    public bool IsStatic { get; set; }
    public string ImageUrl { get; }

    public Alert(string title, string subHeading, string body, string severity, DateTime sunriseDate,
        DateTime sunsetDate, string slug, bool isStatic, string imageUrl)
    {
        Title = title;
        SubHeading = subHeading;
        Body = body;
        Severity = severity;
        SunriseDate = sunriseDate;
        SunsetDate = sunsetDate;
        Slug = slug;
        IsStatic = isStatic;
        ImageUrl = imageUrl;
    }
}