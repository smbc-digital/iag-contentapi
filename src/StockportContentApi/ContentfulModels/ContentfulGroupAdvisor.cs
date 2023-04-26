namespace StockportContentApi.ContentfulModels;

public class ContentfulGroupAdvisor
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public IEnumerable<ContentfulReference> Groups { get; set; } = new List<ContentfulReference>();
    public bool GlobalAccess { get; set; } = false;
}
