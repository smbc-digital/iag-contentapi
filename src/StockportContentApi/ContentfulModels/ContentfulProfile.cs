namespace StockportContentApi.ContentfulModels;

public class ContentfulProfile : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public List<ContentfulInlineQuote> InlineQuotes { get; set; } = new();
    public Asset Image { get; set; } = new() { File = new() { Url = string.Empty }, SystemProperties = new() { Type = "Asset" } };
    public string Body { get; set; } = string.Empty;
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public SystemProperties Sys { get; set; } = new();
    public string TriviaSubheading { get; set; }
    public List<ContentfulTrivia> TriviaSection { get; set; } = new();
    public string Author { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public ContentfulEventBanner EventsBanner { get; set; } = new();
}