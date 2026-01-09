namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulPublicationPage : ContentfulReference
{
    public Asset HeroImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public List<ContentfulPublicationSection> PublicationSections { get; set; } = new();
    public Contentful.Core.Models.Document Body { get; set; }
}