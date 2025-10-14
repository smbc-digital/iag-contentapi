namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulEventHomepage : IContentfulModel
{
    public string TagOrCategory1 { get; set; } = string.Empty;
    public string TagOrCategory2 { get; set; } = string.Empty;
    public string TagOrCategory3 { get; set; } = string.Empty;
    public string TagOrCategory4 { get; set; } = string.Empty;
    public string TagOrCategory5 { get; set; } = string.Empty;
    public string TagOrCategory6 { get; set; } = string.Empty;
    public string TagOrCategory7 { get; set; } = string.Empty;
    public string TagOrCategory8 { get; set; } = string.Empty;
    public string TagOrCategory9 { get; set; } = string.Empty;
    public string TagOrCategory10 { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new();
    public string MetaDescription { get; set; } = string.Empty;
    public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    public IEnumerable<ContentfulAlert> GlobalAlerts { get; set; } = new List<ContentfulAlert>();
    public ContentfulCallToActionBanner CallToAction { get; set; } = null;
    public ContentfulMetadata Metadata { get; set; } = new();
}