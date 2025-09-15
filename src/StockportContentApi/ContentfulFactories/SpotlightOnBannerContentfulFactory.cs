namespace StockportContentApi.ContentfulFactories;

public class SpotlightOnBannerContentfulFactory : IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner>
{
    public SpotlightOnBanner ToModel(ContentfulSpotlightOnBanner entry)
    {
        if (entry is null)
            return null;

        MediaAsset image = new();

        if (entry is not null && entry.Image is not null && entry.Image.File is not null)
        {
            image = new MediaAsset
            {
                Url = entry.Image.File.Url,
                Description = entry.Image.Description
            };
        }

        DateTime updatedAt = entry.Sys.UpdatedAt is not null
            ? entry.Sys.UpdatedAt.Value
            : entry.Sys.PublishedAt.Value;

        return new(entry.Title, image, entry.Teaser, entry.Link, updatedAt);
    }
}