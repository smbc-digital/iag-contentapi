using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using System;

namespace StockportContentApiTests.Unit.Builders
{
    class ContentfulCarouselContentBuilder
    {
        private string _title = "title";
        private string _slug = "slug";
        private string _teaser = "teaser";
        private Asset _image = new Asset { File = new File { Url = "image.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
        private DateTime _sunriseDate = new DateTime(2016, 9, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(2016, 9, 30, 0, 0, 0, DateTimeKind.Utc);
        private string _url = "url";
        private SystemProperties _sys = new SystemProperties { Type = "Entry" };

        public ContentfulCarouselContent Build()
        {
            return new ContentfulCarouselContent()
            {
                Title = _title,
                Slug = _slug,
                Teaser = _teaser,
                Image = _image,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate,
                Url = _url,
                Sys = _sys
            };
        }


    }
}
