namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class CallToActionBanner
{
    public string Title { get; set; } = string.Empty;
    public string Image { get; set; } = null;
    public string Link { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public EColourScheme Colour { get; set; } = EColourScheme.Teal;
}