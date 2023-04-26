namespace StockportContentApi.ContentfulFactories;

public class SpotlightBannerContentfulFactory : IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>
{
    public SpotlightBanner ToModel(ContentfulSpotlightBanner entry)
    {
        return new SpotlightBanner(entry.Title, entry.Teaser, entry.Link);
    }
}
