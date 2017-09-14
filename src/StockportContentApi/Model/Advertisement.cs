using System;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    public class Advertisement
    {
        public string Title { get; }
        public string Slug { get; set; }
        public string Teaser { get; }
        public DateTime SunriseDate { get; }
        public DateTime SunsetDate { get; }
        public bool Isadvertisement { get;}
        public string NavigationUrl { get; }
        public string Image { get; }

        public Advertisement(string title, string slug, string teaser, DateTime sunriseDate, DateTime sunsetDate, bool isadvertisement, string navigationUrl, string image)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            Isadvertisement = isadvertisement;
            NavigationUrl = navigationUrl;
            Image = image;
        }
    }
    
    public class NullAdvertisement : Advertisement
    {
        public NullAdvertisement() : base(string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MinValue, true, string.Empty, String.Empty) { }
    }

}
