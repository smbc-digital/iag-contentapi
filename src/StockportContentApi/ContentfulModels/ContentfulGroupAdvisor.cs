namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulGroupAdvisor
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IEnumerable<ContentfulReference> Groups { get; set; } = new List<ContentfulReference>();
    public bool GlobalAccess { get; set; } = false;
}