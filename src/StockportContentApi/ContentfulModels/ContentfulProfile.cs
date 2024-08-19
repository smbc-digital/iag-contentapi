namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulProfile : ContentfulReference
{
    public string Subtitle { get; set; } = string.Empty;
    public List<ContentfulInlineQuote> InlineQuotes { get; set; } = new();
    public string ImageCaption { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
    public List<ContentfulAlert> InlineAlerts { get; set; } = new();
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public string TriviaSubheading { get; set; }
    public List<ContentfulTrivia> TriviaSection { get; set; } = new();
    public string Author { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public ContentfulEventBanner EventsBanner { get; set; } = new();
    public EColourScheme Colour { get; set; } = EColourScheme.Teal;
}