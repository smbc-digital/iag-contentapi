using System;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    public class Advertisment
    {
        public string Title { get; }
        public string Slug { get; set; }
        public string Teaser { get; }
        public DateTime SunriseDate { get; }
        public DateTime SunsetDate { get; }
        public bool IsAdvertisment { get;}
        public string NavigartionUrl { get; }
        public string Image { get; }

        public Advertisment(string title, string slug, string teaser, DateTime sunriseDate, DateTime sunsetDate, bool isAdvertisment, string navigartionUrl, string image)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            IsAdvertisment = isAdvertisment;
            NavigartionUrl = navigartionUrl;
            Image = image;
        }

        public class NullAdvertisment : Advertisment
        {
            public NullAdvertisment() : base(string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MinValue, true, string.Empty, String.Empty) { }
        }
    }

}
