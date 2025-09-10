namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulNews : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public Asset Image { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public Asset HeroImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public Asset TeaserImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public string HeroImageCaption { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime PublishingDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime SunsetDate { get; set; } = DateTime.MaxValue;
    public List<string> Tags { get; set; } = new();
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public List<Asset> Documents { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public SystemProperties Sys { get; set; } = new();
    public List<ContentfulInlineQuote> InlineQuotes { get; set; } = new();
    public ContentfulCallToActionBanner CallToAction { get; init; }
    public ContentfulTrustedLogo FeaturedLogo { get; set; }
    public string LogoAreaTitle { get; set; }
    public List<ContentfulTrustedLogo> TrustedLogos { get; set; } = new();
    public string EventsByTagOrCategory { get; set; } = string.Empty;
    public List<ContentfulEvent> Events { get; set; } = new();
}