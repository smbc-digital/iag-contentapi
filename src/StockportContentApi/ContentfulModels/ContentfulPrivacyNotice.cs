namespace StockportContentApi.ContentfulModels;

public class ContentfulPrivacyNotice : ContentfulReference
{
    public new string Slug { get; set; }
    public new string Title { get; set; }
    public string Category { get; set; }
    public bool OutsideEu { get; set; }
    public bool AutomatedDecision { get; set; }
    public string Purpose { get; set; }
    public string TypeOfData { get; set; }
    public string Legislation { get; set; }
    public string Obtained { get; set; }
    public string ExternallyShared { get; set; }
    public string RetentionPeriod { get; set; }
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
    public new SystemProperties Sys { get; set; }

    public ContentfulPrivacyNotice() { }
}