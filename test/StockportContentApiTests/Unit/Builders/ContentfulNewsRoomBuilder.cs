namespace StockportContentApiTests.Unit.Builders;

public class ContentfulNewsRoomBuilder
{
    private readonly List<ContentfulAlert> _alerts = new() { new ContentfulAlertBuilder().Build() };
    private readonly SystemProperties _sys = new() { Type = "Entry" };

    public ContentfulNewsRoom Build()
        => new()
        {
            Title = "title",
            Alerts = _alerts,
            EmailAlertsTopicId = "test-id",
            EmailAlerts = true,
            Sys = _sys
        };
}