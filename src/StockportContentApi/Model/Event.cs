using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class Event
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Teaser { get; set; }
        public string Image { get; set; }     
        public string ThumbnailImage { get; set; }     
        public string Description { get; set; }
        public DateTime SunriseDate { get; set; }
        public DateTime SunsetDate { get; set; }
        public string Fee { get; set; }
        public string Location { get; set; }
        public string SubmittedBy { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool Featured { get; set; }
        public DateTime EventDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<Crumb> Breadcrumbs { get; set; }

        public Event() {}

        public Event(string title, string slug, string teaser, string image, string thumbnailImage, string description, DateTime sunriseDate,
            DateTime sunsetDate, string fee, string location, string submittedBy, string longitude, string latitude, bool featured, DateTime eventDate, string startTime, string endTime, List<Crumb> breadcrumbs) 
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Image = image;
            Description = description;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            ThumbnailImage = thumbnailImage;
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
    }

    public class NullEvent : Event
    {
        public NullEvent()
            : base(
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new DateTime(), new DateTime(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                false, new DateTime(), string.Empty, string.Empty, new List<Crumb>())
        { }
    }
}
