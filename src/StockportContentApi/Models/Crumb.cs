namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Crumb(string title, string slug, string type)
{
    public string Title { get; } = title;
    public string Slug { get; } = slug;
    public string Type { get; } = type;
}