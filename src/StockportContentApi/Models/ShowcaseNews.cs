namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class ShowcaseNews
{
    public string Type { get; set; } = string.Empty;

    public News News { get; set; } = null;
}