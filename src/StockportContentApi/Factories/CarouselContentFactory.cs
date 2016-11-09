using System;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class CarouselContentFactory : IFactory<CarouselContent>
    {
        public CarouselContent Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            var title = (string)entry.fields.title ?? string.Empty;
            var slug = (string)entry.fields.slug ?? string.Empty;
            var teaser = (string)entry.fields.teaser ?? string.Empty;

            var image = contentfulResponse.GetImageUrl(entry.fields.image);

            var url = (string) entry.fields.url ?? string.Empty;

            DateTime sunriseDate;
            DateTime.TryParse((string)entry.fields.sunriseDate ?? string.Empty, out sunriseDate);
            DateTime sunsetDate;
            DateTime.TryParse((string)entry.fields.sunsetDate ?? string.Empty, out sunsetDate);

            return new CarouselContent(title, slug, teaser, image, sunriseDate, sunsetDate, url);
        }
    }
}
