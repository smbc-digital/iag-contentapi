namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulFilter : ContentfulReference
{
    public string DisplayName { get; set; }
    public string Theme { get; set; }
}