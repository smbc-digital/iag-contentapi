namespace StockportContentApi.Model;

[ExcludeFromCodeCoverage]
public class Article
{
    public string Body { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Teaser { get; set; }
    public string MetaDescription { get; set; }
    public string Icon { get; set; }
    public string BackgroundImage { get; set; }
    public string Image { get; set; }
    public string AltText { get; set; }
    public List<Section> Sections { get; set; }
    public IEnumerable<Crumb> Breadcrumbs { get; set; }
    public IEnumerable<Alert> Alerts { get; set; }
    public IEnumerable<Alert> AlertsInline { get; set; }
    public IEnumerable<Profile> Profiles { get; set; }
    public string LogoAreaTitle { get; set; }
    public List<GroupBranding> ArticleBranding { get; set; }
    public Topic ParentTopic { get; set; }
    public List<Document> Documents { get; set; }
    public DateTime SunriseDate { get; set; }
    public DateTime SunsetDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool HideLastUpdated { get; set; }
}