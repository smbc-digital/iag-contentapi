namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class EventBanner(string title, string teaser, string icon, string link, EColourScheme colour)
{
    public string Title { get; } = title;
    public string Teaser { get; } = teaser;
    public string Icon { get; } = icon;
    public string Link { get; } = link;
    public EColourScheme Colour { get; } = colour;
}

public class NullEventBanner : EventBanner
{
    public NullEventBanner() : base(string.Empty, string.Empty, string.Empty, string.Empty, EColourScheme.None) { }
}