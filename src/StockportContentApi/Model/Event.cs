using System;
using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Event
    {
        public string Title { get; }
        public string Slug { get; }
        public string Teaser { get; }
        public string ImageUrl { get; }
        public string ThumbnailImageUrl { get; }
        public string Description { get; }
        public string Fee { get; }
        public string Location { get; }
        public string SubmittedBy { get; }
        public string Longitude { get; }
        public string Latitude { get; }
        public bool Featured { get; }
        public DateTime EventDate { get; }
        public string StartTime { get; }
        public string EndTime { get; }
        public int Occurences { get; } 
        public EventFrequency Frequency { get; }
        public List<Crumb> Breadcrumbs { get; }
        public List<Document> Documents { get; }

        public Event(string title, string slug, string teaser, string imageUrl, string description, string fee, 
                     string location, string submittedBy, string longitude, string latitude, bool featured, DateTime eventDate, string startTime, 
                     string endTime, int occurences, EventFrequency frequency, List<Crumb> breadcrumbs, string thumbnailImageUrl, List<Document> documents ) 
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
            ThumbnailImageUrl = thumbnailImageUrl;
            ImageUrl = imageUrl;
            Documents = documents;
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
