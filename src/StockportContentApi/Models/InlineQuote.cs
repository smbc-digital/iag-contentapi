namespace StockportContentApi.Model;

public class InlineQuote
{
    public string Image { get; set; } = null;
    public string ImageAltText { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
}
