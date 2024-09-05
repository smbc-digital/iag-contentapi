namespace StockportContentApi.Model;

[ExcludeFromCodeCoverage]
public class LandingPage
{
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public IEnumerable<Crumb> Breadcrumbs { get; set; }
    public IEnumerable<Alert> Alerts { get; set; }
    public string Teaser { get; set; }
    public string MetaDescription { get; set; }
    public MediaAsset Image { get; set; }
    public string HeaderType { get; set; }
    public MediaAsset HeaderImage { get; set; }
    public EColourScheme HeaderColourScheme { get; set; }
    public IEnumerable<SubItem> ContentReference { get; set; }
}