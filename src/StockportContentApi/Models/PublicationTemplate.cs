namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class PublicationTemplate
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string MetaDescription { get; set; }
    public string Subtitle { get; set; }
    public MediaAsset HeroImage { get; set; }
    public bool DisplayReviewDate { get; set; } = true;
    public List<PublicationPage> PublicationPages { get; set; } = new();
    public EColourScheme ColourScheme { get; set; }
}