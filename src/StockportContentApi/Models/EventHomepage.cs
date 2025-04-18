﻿namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class EventHomepageRow
{
    public bool IsLatest { get; set; }
    public string Tag { get; set; }
    public bool MatchedByTag { get; set; }
    public IEnumerable<Event> Events { get; set; }
}

[ExcludeFromCodeCoverage]
public class EventHomepage(IEnumerable<EventHomepageRow> rows)
{
    public IEnumerable<EventHomepageRow> Rows { get; } = rows;
    public IEnumerable<EventCategory> Categories { get; set; }
    public List<Alert> Alerts { get; set; } = new();
    public List<Alert> GlobalAlerts { get; set; } = new();
    public CallToActionBanner CallToAction { get; set; }
    public string MetaDescription { get; set; }
}