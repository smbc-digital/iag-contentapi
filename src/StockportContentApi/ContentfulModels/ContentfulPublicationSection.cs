namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulPublicationSection : ContentfulReference
{
    public Contentful.Core.Models.Document Body { get; set; }
}