namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class EventHomepageRow
{
    public bool IsLatest { get; set; }
    public string Tag { get; set; }
    public IEnumerable<Event> Events { get; set; }
}

[ExcludeFromCodeCoverage]
public class EventHomepage(IEnumerable<EventHomepageRow> rows)
{
    public IEnumerable<EventHomepageRow> Rows { get; } = rows;
    public IEnumerable<EventCategory> Categories { get; set; }
    public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    public CallToActionBanner CallToAction { get; set; }
    public string MetaDescription { get; set; }
}