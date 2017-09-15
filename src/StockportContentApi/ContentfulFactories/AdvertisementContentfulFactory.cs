using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class AdvertisementContentfulFactory : IContentfulFactory<ContentfulAdvertisement, Advertisement>
    {
        public Advertisement ToModel(ContentfulAdvertisement entry)
        {

            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                ? entry.Image.File.Url
                : string.Empty;

            return new Advertisement(entry.Title, entry.Slug, entry.Teaser, entry.SunriseDate, entry.SunriseDate,
                entry.IsAdvertisement, entry.NavigationUrl, imageUrl);
        }
    }
}