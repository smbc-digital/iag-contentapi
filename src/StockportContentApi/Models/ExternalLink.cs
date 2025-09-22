namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class ExternalLink(string title, string url, string teaser)
{
    public string Title { get; set; } = title;
    public string URL { get; set; } = url;
    public string Teaser { get; set; } = teaser;
}