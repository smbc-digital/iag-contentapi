namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Video(string heading, string text, string videoEmbedCode)
{
    public string Heading { get; set; } = heading;
    public string Text { get; set; } = text;
    public string VideoEmbedCode { get; set; } = videoEmbedCode;
}