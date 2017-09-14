using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulAdvertisementBuilder
    {
        public string _title { get; set; } = "Advert Title";
        public string _slug { get; set; } = "advert slug";
        public string _teaser { get; set; } = "advert teaser";
        public DateTime _sunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime _sunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public bool _isAdvertisment { get; set; } = true;
        public string _navigationUrl { get; set; } = string.Empty;
        public Asset _image { get; set; } = new Asset { File = new File { Url = "url" }, SystemProperties = new SystemProperties { Type = "Asset" } };
        
        public ContentfulAdvertisement Build()
        {
            return new ContentfulAdvertisement()
            {
                Image = _image,
                IsAdvertisment = _isAdvertisment,
                NavigationUrl = _navigationUrl,
                Slug = _slug,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate,
                Teaser = _teaser,
                Title = _title
            };
        }
        
        
        
    }
}
