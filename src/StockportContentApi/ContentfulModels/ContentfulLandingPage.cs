namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulLandingPage : ContentfulReference
{
    public string Subtitle { get; set; } = string.Empty;
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
    public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    public string HeaderType { get; set; } = string.Empty;
    public Asset HeaderImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public EColourScheme HeaderColourScheme { get; set; } = EColourScheme.Teal;
    public List<ContentfulReference> PageSections { get; set; } = new();
}