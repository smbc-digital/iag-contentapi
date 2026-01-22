namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulPublicationSection : ContentfulReference
{
    public Contentful.Core.Models.Document Body { get; set; }
    public List<ContentfulAlert> InlineAlerts { get; set; } = new();
    public List<ContentfulInlineQuote> InlineQuotes { get; set; } = new();
    public string LogoAreaTitle { get; set; }
    public List<ContentfulTrustedLogo> TrustedLogos { get; set; } = new();
}