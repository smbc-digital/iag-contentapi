﻿namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class EventHomepageRow
{
    public bool IsLatest { get; set; }
    public string Tag { get; set; }
    public IEnumerable<Event> Events { get; set; }
}

[ExcludeFromCodeCoverage]
public class EventHomepage
{
    public IEnumerable<EventHomepageRow> Rows { get; }
    public IEnumerable<EventCategory> Categories { get; set; }
    public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();

    public string MetaDescription { get; set; }

    public EventHomepage(IEnumerable<EventHomepageRow> rows)
    {
        Rows = rows;
    }
}