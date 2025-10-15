namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Section
{
    public string Title { get; }
    public string Slug { get; }
    public string MetaDescription { get; }
    public string Body { get; set; }
    public IEnumerable<Profile> Profiles { get; } = new List<Profile>();
    public IEnumerable<Alert> AlertsInline { get; set; }
    public List<Document> Documents { get; }
    public string LogoAreaTitle { get; set; }
    public List<TrustedLogo> TrustedLogos { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<InlineQuote> InlineQuotes { get; set; }
    public List<string> Websites { get; set; }

    public Section(string title,
                string slug,
                string metaDescription,
                string body,
                IEnumerable<Profile> profiles,
                List<Document> documents,
                string logoAreaTitle,
                List<TrustedLogo> trustedLogos,
                DateTime updatedAt,
                IEnumerable<Alert> alertsInline,
                List<InlineQuote> inlineQuotes,
                List<string> websites)
    {
        Title = title;
        Slug = slug;
        MetaDescription = metaDescription;
        Body = body;
        Profiles = profiles;
        Documents = documents;
        LogoAreaTitle = logoAreaTitle;
        TrustedLogos = trustedLogos;
        UpdatedAt = updatedAt;
        AlertsInline = alertsInline;
        InlineQuotes = inlineQuotes;
        Websites = websites;
    }

    public Section()
    { }
}