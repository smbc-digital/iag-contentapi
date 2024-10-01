namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Newsroom
{
    public readonly List<Alert> Alerts;
    public readonly bool EmailAlerts;
    public readonly string EmailAlertsTopicId;
    public List<News> News { get; private set; }
    public List<string> Categories { get; private set; }
    public List<DateTime> Dates { get; private set; }

    public Newsroom(List<Alert> alerts, bool emailAlerts, string emailAlertsTopicId)
    {
        Alerts = alerts;
        EmailAlerts = emailAlerts;
        EmailAlertsTopicId = emailAlertsTopicId;
    }

    public void SetNews(List<News> news)
    {
        News = news;
    }

    public void SetCategories(List<string> categories)
    {
        Categories = categories;
    }

    public void SetDates(List<DateTime> dates)
    {
        Dates = dates;
    }
}