namespace StockportContentApi.ContentfulFactories;

public class SpotlightBannerContentfulFactory : IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>
{
    public SpotlightBanner ToModel(ContentfulSpotlightBanner entry) =>
        new(entry.Title, entry.Teaser, entry.Link);
}
