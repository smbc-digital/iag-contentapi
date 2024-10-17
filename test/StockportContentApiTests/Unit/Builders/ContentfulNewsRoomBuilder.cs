namespace StockportContentApiTests.Unit.Builders;

public class ContentfulNewsRoomBuilder
{
    private readonly string _title = "title";
    private readonly List<ContentfulAlert> _alerts = new() { new ContentfulAlertBuilder().Build() };
    private readonly bool _emailAlerts = true;
    private readonly string _emailAlertsTopicId = "test-id";
    private readonly SystemProperties _sys = new() { Type = "Entry" };

    public ContentfulNewsRoom Build()
        => new()
        {
            Title = _title,
            Alerts = _alerts,
            EmailAlertsTopicId = _emailAlertsTopicId,
            EmailAlerts = _emailAlerts,
            Sys = _sys
        };
}