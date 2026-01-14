namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class PublicationPage
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string MetaDescription { get; set; }
    public List<PublicationSection> PublicationSections { get; set; } = new();
    public Contentful.Core.Models.Document Body { get; set; }
}