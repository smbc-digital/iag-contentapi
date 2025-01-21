namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class TrustedLogos
{
    public string Title { get; set; }

    public string Text { get; set; }

    public MediaAsset File { get; set; }

    public string Url { get; set; }

    public TrustedLogos(string title, string text, MediaAsset file, string url)
    {
        Title = title;
        Text = text;
        File = file;
        Url = url;
    }
}