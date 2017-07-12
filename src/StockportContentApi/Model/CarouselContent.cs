using Contentful.Core.Models;
using System;

namespace StockportContentApi.Model
{
    public class CarouselContent
    {
        public string Title { get; }
        public string Slug { get;}
        public string Teaser { get; }
        public string Image { get; }
        public DateTime SunriseDate { get; }
        public DateTime SunsetDate { get; }
        public string Url { get; }

        public CarouselContent(string title, string slug, string teaser, string image, DateTime sunriseDate, DateTime sunsetDate, string url)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Image = image;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            Url = url;
        }
    }
}
