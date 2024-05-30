namespace StockportContentApi.Model;
[ExcludeFromCodeCoverage]
public class EventBanner
{
    public string Title { get; }
    public string Teaser { get; }
    public string Icon { get; }
    public string Link { get; }
    public string Colour { get; }

    public EventBanner(string title, string teaser, string icon, string link, string colour)
    {
        Title = title;
        Teaser = teaser;
        Icon = icon;
        Link = link;
        Colour = colour;
    }
}

public class NullEventBanner : EventBanner
{
    public NullEventBanner() : base(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) { }
}