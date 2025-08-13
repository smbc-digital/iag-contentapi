namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulArticle : ContentfulReference
{
    public string AltText { get; set; }
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public List<ContentfulProfile> Profiles { get; set; } = new();
    public List<Asset> Documents { get; set; } = new();
    public List<ContentfulAlert> AlertsInline { get; set; } = new();
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
    public List<ContentfulReference> RelatedContent { get; set; } = new();
    public List<ContentfulTrustedLogo> TrustedLogos { get; set; } = new();
    public string LogoAreaTitle { get; set; }
    public string Author { get; set; }
    public string Photographer { get; set; }
    public List<ContentfulInlineQuote> InlineQuotes { get; set; } = new();
    public List<ContentfulCallToActionBanner> CallToActionBanners { get; set; } = new();
}