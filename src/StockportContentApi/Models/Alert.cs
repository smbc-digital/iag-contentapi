namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Alert(string title,
                string body,
                string severity,
                DateTime sunriseDate,
                DateTime sunsetDate,
                string slug,
                bool isStatic,
                string imageUrl)
{
    public string Title { get; } = title;
    public string Body { get; } = body;
    public string Severity { get; } = severity;
    public DateTime SunriseDate { get; } = sunriseDate;
    public DateTime SunsetDate { get; } = sunsetDate;
    public string Slug { get; set; } = slug;
    public bool IsStatic { get; set; } = isStatic;
    public string ImageUrl { get; } = imageUrl;
}