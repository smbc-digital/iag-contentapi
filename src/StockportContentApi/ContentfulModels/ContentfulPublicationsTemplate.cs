namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulPublicationTemplate : ContentfulReference
{
    public Asset HeroImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public string Subtitle { get; set; } = string.Empty;
    public Contentful.Core.Models.Document Body { get; set; }
    public bool DisplayReviewDate { get; set; } = true;
    public List<ContentfulPublicationPage> PublicationPages { get; set; } = new();
}