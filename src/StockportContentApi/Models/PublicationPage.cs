namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class PublicationPage
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string MetaDescription { get; set; }
    public List<PublicationSection> PublicationSections { get; set; }
    public Contentful.Core.Models.Document Body { get; set; }
    public IEnumerable<Alert> InlineAlerts { get; set; }
    public List<InlineQuote> InlineQuotes { get; set; }
    public string LogoAreaTitle { get; set; }
    public List<TrustedLogo> TrustedLogos { get; set; }
}