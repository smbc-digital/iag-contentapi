namespace StockportContentApi.ContentfulModels;

/// <summary>
/// Contentful reference base class, used for anything that is generically referenced
/// </summary>
public class ContentfulReference : IContentfulModel
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ColourScheme { get; set; } = "Teal";
    public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
    public bool Highlight { get; set; } = false;
    public bool HideLastUpdated { get; set; } = false;
    public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public Asset BackgroundImage { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public List<ContentfulReference> SubItems { get; set; } = new List<ContentfulReference>();
    public List<ContentfulReference> SecondaryItems { get; set; } = new List<ContentfulReference>();
    public List<ContentfulReference> TertiaryItems { get; set; } = new List<ContentfulReference>();
    public List<ContentfulSection> Sections { get; set; } = new List<ContentfulSection>();
    public SystemProperties Sys { get; set; } = new SystemProperties();
}
