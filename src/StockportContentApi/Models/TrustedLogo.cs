namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class TrustedLogo(string title, string text, MediaAsset image, string link, List<string> websites)
{
    public string Title { get; set; } = title;
    public string Text { get; set; } = text;
    public MediaAsset Image { get; set; } = image;
    public string Link { get; set; } = link;
    public List<string> Websites { get; set;} = websites;
}