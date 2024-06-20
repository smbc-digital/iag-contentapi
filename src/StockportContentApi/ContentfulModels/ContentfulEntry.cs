namespace StockportContentApi.ContentfulModels;

public class ContentfulEntry : ContentfulReference
{
    public IDictionary<string, dynamic> Content { get; set; } = new Dictionary<string, dynamic>();
}