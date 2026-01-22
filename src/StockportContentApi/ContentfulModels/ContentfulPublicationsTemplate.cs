namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulPublicationTemplate : ContentfulReference
{
    public string Summary { get; set; } = string.Empty;
    public Asset HeaderImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public string Subtitle { get; set; } = string.Empty;
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
    public DateTime DatePublished { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime ReviewDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
    public string PublicationTheme { get; set; } = string.Empty;
    public List<ContentfulPublicationPage> PublicationPages { get; set; } = new();
    public string LogoAreaTitle { get; set; } = string.Empty;
    public List<ContentfulTrustedLogo> TrustedLogos { get; set; } = new();
}