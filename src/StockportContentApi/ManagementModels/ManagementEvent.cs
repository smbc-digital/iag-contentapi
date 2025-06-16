namespace StockportContentApi.ManagementModels;

public class ManagementEvent
{
    public Dictionary<string, string> Title { get; set; }
    public Dictionary<string, string> Slug { get; set; }
    public Dictionary<string, string> Teaser { get; set; }
    public Dictionary<string, LinkReference> Image { get; set; }
    public Dictionary<string, string> Description { get; set; }
    public Dictionary<string, string> Fee { get; set; }
    public Dictionary<string, string> Location { get; set; }
    public Dictionary<string, string> SubmittedBy { get; set; }
    public Dictionary<string, DateTime> EventDate { get; set; }
    public Dictionary<string, string> StartTime { get; set; }
    public Dictionary<string, string> EndTime { get; set; }
    public Dictionary<string, int> Occurrences { get; set; }
    public Dictionary<string, EventFrequency> Frequency { get; set; }
    public Dictionary<string, List<LinkReference>> Documents { get; set; }
    public Dictionary<string, MapPosition> MapPosition { get; set; }
    public Dictionary<string, string> BookingInformation { get; set; }
    public Dictionary<string, bool> Featured { get; set; }
    public Dictionary<string, List<string>> Tags { get; set; }
    public Dictionary<string, List<ManagementAlert>> Alerts { get; set; }
    public Dictionary<string, ManagementReference> Group { get; set; }
    public Dictionary<string, List<ManagementEventCategory>> EventCategories { get; set; }
    public Dictionary<string, bool?> Free { get; set; }
    public Dictionary<string, bool?> Paid { get; set; }
}
