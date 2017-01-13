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
        public string ImageUrl { get; set; } = string.Empty;
        public string ThumbnailImageImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Fee { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public bool Featured { get; set; } = false;
        public DateTime EventDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int Occurences { get; set; } = 0;
        public EventFrequency Frequency { get; set; } = EventFrequency.None;
        public List<Crumb> Breadcrumbs { get; set; } = new List<Crumb> { new Crumb("Events", string.Empty, "events") };

        public Event() {}

        public Event(string title, string slug, string teaser, string imageUrl, string description, string fee, 
                     string location, string submittedBy, string longitude, string latitude, bool featured, DateTime eventDate, string startTime, 
                     string endTime, int occurences, EventFrequency frequency, List<Crumb> breadcrumbs, string thumbnailImageUrl) 
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Description = description;
            Fee = fee;
            Location = location;
            SubmittedBy = submittedBy;
            Longitude = longitude;
            Latitude = latitude;
            Featured = featured;
            EventDate = eventDate;
            StartTime = startTime;
            EndTime = endTime;
            Occurences = occurences;
            Frequency = frequency;
            Breadcrumbs = breadcrumbs;
            ThumbnailImageImageUrl = thumbnailImageUrl;
            ImageUrl = imageUrl;
        }

        public bool ShouldSerializeImageAsset()
        {
            return false;
        }

        public bool ShouldSerializeFrequency()
        {
            return false;
        }

        public bool ShouldSerializeOccurences()
        {
            return false;
        }
    }
}
