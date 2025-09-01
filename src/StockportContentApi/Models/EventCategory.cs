namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]

public class EventCategory(string name, string slug, string icon, string image)
{
    public string Name { get; } = name;
    public string Slug { get; } = slug;
    public string Icon { get; } = icon;
    public string Image { get; } = image;
}