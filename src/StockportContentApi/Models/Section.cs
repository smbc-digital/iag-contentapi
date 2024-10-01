namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Section
{
    public string Title { get; }
    public string Slug { get; }
    public string MetaDescription { get; }
    public string Body { get; set; }
    public IEnumerable<Profile> Profiles { get; } = new List<Profile>();
    public IEnumerable<Alert> AlertsInline { get; }
    public List<Document> Documents { get; }
    public string LogoAreaTitle { get; set; }
    public List<GroupBranding> SectionBranding { get; set; }
    public DateTime SunriseDate { get; }
    public DateTime SunsetDate { get; }
    public DateTime UpdatedAt { get; set; }

    public Section(string title, string slug, string metaDescription, string body, IEnumerable<Profile> profiles, List<Document> documents, string logoAreaTitle, List<GroupBranding> sectionBranding, DateTime sunriseDate, DateTime sunsetDate, DateTime updatedAt, IEnumerable<Alert> alertsInline)
    {
        Title = title;
        Slug = slug;
        MetaDescription = metaDescription;
        Body = body;
        Profiles = profiles;
        Documents = documents;
        LogoAreaTitle = logoAreaTitle;
        SectionBranding = sectionBranding;
        SunriseDate = sunriseDate;
        SunsetDate = sunsetDate;
        UpdatedAt = updatedAt;
        AlertsInline = alertsInline;
    }
}