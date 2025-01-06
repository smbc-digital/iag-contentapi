namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class EventCalender
{
    public List<Event> Events { get; private set; }
    public List<Event> FeaturedEvents { get; set; }
    public List<string> Categories { get; private set; }

    public void SetEvents(List<Event> events, List<string> categories, List<Event> featuredEvents)
    {
        Events = events;
        Categories = categories;
        FeaturedEvents = featuredEvents;
    }
}