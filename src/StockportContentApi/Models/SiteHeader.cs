namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class SiteHeader(string title, List<SubItem> items, string logo)
{
    public string Title { get; set; } = title;

    public List<SubItem> Items { get; set; } = items;

    public string Logo { get; set; } = logo;
}