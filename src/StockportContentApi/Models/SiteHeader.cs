namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class SiteHeader
{
    public SiteHeader(string title, List<SubItem> items, string logo)
    {
        Title = title;
        Items = items;
        Logo  = logo;
    }

    public string Title { get; set; } = string.Empty;

    public List<SubItem> Items { get; set; } = new();

    public string Logo { get; set; }
}