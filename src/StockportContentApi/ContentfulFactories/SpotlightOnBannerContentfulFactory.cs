namespace StockportContentApi.ContentfulFactories;

public class SpotlightOnBannerContentfulFactory : IContentfulFactory<IEnumerable<ContentfulSpotlightOnBanner>, IEnumerable<SpotlightOnBanner>>
{
    public IEnumerable<SpotlightOnBanner> ToModel(IEnumerable<ContentfulSpotlightOnBanner> entry)
    {

        return entry.Select(_ => new SpotlightOnBanner(_.Title, _.Image.File.Url, _.AltText, _.Teaser, _.Link));
    }
}
