namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class SpotlightOnBanner(string title,
                            MediaAsset image,
                            string teaser,
                            string link,
                            DateTime lastUpdated)
{
    public string Title { get; } = title;
    public MediaAsset Image { get; } = image;
    public string Teaser { get; } = teaser;
    public string Link { get; } = link;
    public DateTime LastUpdated { get; } = lastUpdated;
}