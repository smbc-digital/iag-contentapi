namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Newsroom(List<Alert> alerts, bool emailAlerts, string emailAlertsTopicId, CallToActionBanner callToAction)
{
    public readonly List<Alert> Alerts = alerts;
    public readonly bool EmailAlerts = emailAlerts;
    public readonly string EmailAlertsTopicId = emailAlertsTopicId;
    public List<News> News { get; private set; }
    public List<string> Categories { get; private set; }
    public List<DateTime> Dates { get; private set; }
    public CallToActionBanner CallToAction { get; init; } = callToAction;

    public void SetNews(List<News> news) =>
        News = news;

    public void SetCategories(List<string> categories) =>
        Categories = categories;

    public void SetDates(List<DateTime> dates) =>
        Dates = dates;
}