namespace StockportContentApi.ContentfulModels;

public class ContentfulInlineQuote
{
    public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };

    public string ImageAltText { get; set; } = string.Empty;

    public string Quote { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Theme { get; set; } = string.Empty;
}
