namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulPublicationTemplate : ContentfulReference
{
    public Asset HeroImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public string Subtitle { get; set; } = string.Empty;
    public DateTime DatePublished { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime LastUpdated { get; set; } = DateTime.MaxValue.ToUniversalTime();
    public List<ContentfulPublicationPage> PublicationPages { get; set; } = new();
}