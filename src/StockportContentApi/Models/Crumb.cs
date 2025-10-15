namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Crumb(string title, string slug, string type, List<string> websites)
{
    public string Title { get; } = title;
    public string Slug { get; } = slug;
    public string Type { get; } = type;
    public List<string> Websites { get; } = websites;
}