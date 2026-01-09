namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class PublicationSection
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string MetaDescription { get; set; }
    public Contentful.Core.Models.Document Body { get; set; }
}