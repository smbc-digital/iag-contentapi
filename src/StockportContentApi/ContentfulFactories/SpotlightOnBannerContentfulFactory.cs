﻿namespace StockportContentApi.ContentfulFactories;

public class SpotlightOnBannerContentfulFactory : IContentfulFactory<IEnumerable<ContentfulSpotlightOnBanner>, IEnumerable<SpotlightOnBanner>>
{
    public IEnumerable<SpotlightOnBanner> ToModel(IEnumerable<ContentfulSpotlightOnBanner> entry)
    {
        return entry.Select(_ => new SpotlightOnBanner(_.Title, _.Image.File.Url, _.AltText, _.Teaser, _.Link, _.Sys.UpdatedAt is not null ? _.Sys.UpdatedAt.Value : _.Sys.PublishedAt.Value));
    }
}
