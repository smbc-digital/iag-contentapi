using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class Event
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        [JsonProperty(Required = Required.Default)]
        public string ImageUrl => ImageAsset?.File?.Url ?? string.Empty;
        [JsonProperty("image")]
        public Asset ImageAsset { get; set; } = new Asset { File = new File { Url = "" } };
        [JsonProperty(Required = Required.Default)]
        public string ThumbnailImageUrl => ConvertToThumbnail(ImageAsset?.File?.Url);
        public string Description { get; set; } = string.Empty;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public string Fee { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public bool Featured { get; set; } = false;
        public DateTime EventDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public List<Crumb> Breadcrumbs { get; set; } = new List<Crumb> { new Crumb("Events", string.Empty, "events") };

        public Event() {}

        public Event(string title, string slug, string teaser, string imageUrl, string description, DateTime sunriseDate, DateTime sunsetDate, string fee, 
                     string location, string submittedBy, string longitude, string latitude, bool featured, DateTime eventDate, string startTime, 
                     string endTime, List<Crumb> breadcrumbs) 
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            ImageAsset = new Asset { File = new File { Url = imageUrl } };
            Description = description;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            Fee = fee;
            Location = location;
            SubmittedBy = submittedBy;
            Longitude = longitude;
            Latitude = latitude;
            Featured = featured;
            EventDate = eventDate;
            StartTime = startTime;
            EndTime = endTime;
            Breadcrumbs = breadcrumbs;
        }

        private static string ConvertToThumbnail(string thumbnailImage)
        {
            return string.IsNullOrEmpty(thumbnailImage) ? "" : thumbnailImage + "?h=250";
        }

        public bool ShouldSerializeImageAsset()
        {
            return false;
        }
    }
}
