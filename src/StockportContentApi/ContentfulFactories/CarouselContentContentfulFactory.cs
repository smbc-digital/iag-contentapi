using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using System;

namespace StockportContentApi.ContentfulFactories
{
    public class CarouselContentContentfulFactory : IContentfulFactory<ContentfulCarouselContent, CarouselContent>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarouselContentContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CarouselContent ToModel(ContentfulCarouselContent carousel)
        {
            var title = carousel.Title ?? string.Empty;
            var slug = carousel.Slug ?? string.Empty;
            var teaser = carousel.Teaser ?? string.Empty;
            var image = ContentfulHelpers.EntryIsNotALink(carousel.Image.SystemProperties) ? carousel.Image.File.Url : string.Empty;

            var url = carousel.Url ?? string.Empty;

            DateTime sunriseDate = DateComparer.DateFieldToDate(carousel.SunriseDate);
            DateTime sunsetDate = DateComparer.DateFieldToDate(carousel.SunsetDate);

            return new CarouselContent(title, slug, teaser, image, sunriseDate, sunsetDate, url).StripData(_httpContextAccessor);
        }
    }
}