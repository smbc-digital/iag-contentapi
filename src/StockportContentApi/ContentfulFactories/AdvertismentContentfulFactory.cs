using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class AdvertismentContentfulFactory : IContentfulFactory<ContentfulAdvertisment, Advertisment>
    {
        public Advertisment ToModel(ContentfulAdvertisment entry)
        {

            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                ? entry.Image.File.Url
                : string.Empty;

            return new Advertisment(entry.Title, entry.Slug, entry.Teaser, entry.SunriseDate, entry.SunriseDate,
                entry.IsAdvertisment, entry.NavigartionUrl, imageUrl);
        }
    }
}