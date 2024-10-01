namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class EventBanner
{
    public string Title { get; }
    public string Teaser { get; }
    public string Icon { get; }
    public string Link { get; }
    public EColourScheme Colour { get; }

    public EventBanner(string title, string teaser, string icon, string link, EColourScheme colour)
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
    public NullEventBanner() : base(string.Empty, string.Empty, string.Empty, string.Empty, EColourScheme.None) { }
}