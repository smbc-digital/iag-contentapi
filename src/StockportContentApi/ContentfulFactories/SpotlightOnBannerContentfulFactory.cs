namespace StockportContentApi.ContentfulFactories;

public class SpotlightOnBannerContentfulFactory : IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner>
{
    public SpotlightOnBanner ToModel(ContentfulSpotlightOnBanner entry)
    {
        if (entry is null)
            return null;

        DateTime updatedAt = entry.Sys.UpdatedAt is not null ? entry.Sys.UpdatedAt.Value : entry.Sys.PublishedAt.Value;

        return new(entry.Title, entry?.Image?.File?.Url, entry.AltText, entry.Teaser, entry.Link, updatedAt);
    }
}
