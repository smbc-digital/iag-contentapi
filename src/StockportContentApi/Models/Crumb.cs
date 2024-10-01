namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Crumb
{
    public string Title { get; }
    public string Slug { get; }
    public string Type { get; }

    public Crumb(string title, string slug, string type)
    {
        Title = title;
        Slug = slug;
        Type = type;
    }
}