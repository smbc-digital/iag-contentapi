namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulSection : ContentfulReference
{
    public List<ContentfulProfile> Profiles { get; set; } = new();
    public List<Asset> Documents { get; set; } = new();
    public List<ContentfulAlert> AlertsInline { get; set; } = new();
    public List<ContentfulTrustedLogo> TrustedLogos { get; set; } = new();
    public string LogoAreaTitle { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ContentfulCallToActionBanner> CallToActionBanners { get; set; } = new();
    public List<ContentfulInlineQuote> InlineQuotes { get; set; } = new();
}