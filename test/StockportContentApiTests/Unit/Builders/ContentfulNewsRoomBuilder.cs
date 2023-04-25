namespace StockportContentApiTests.Unit.Builders;

public class ContentfulNewsRoomBuilder
{
    private string _title = "title";
    private List<ContentfulAlert> _alerts = new List<ContentfulAlert> { new ContentfulAlertBuilder().Build() };
    private bool _emailAlerts = true;
    private string _emailAlertsTopicId = "test-id";
    private SystemProperties _sys = new SystemProperties { Type = "Entry" };

    public ContentfulNewsRoom Build()
    {
        return new ContentfulNewsRoom
        {
            Title = _title,
            Alerts = _alerts,
            EmailAlertsTopicId = _emailAlertsTopicId,
            EmailAlerts = _emailAlerts,
            Sys = _sys
        };
    }
}
