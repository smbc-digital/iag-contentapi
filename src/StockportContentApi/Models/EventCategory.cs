namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]

public class EventCategory
{
    public string Name { get; }
    public string Slug { get; }
    public string Icon { get; }
    public string Image { get; }

    public EventCategory(string name, string slug, string icon, string image)
    {
        Name = name;
        Slug = slug;
        Icon = icon;
        Image = image;
    }
}