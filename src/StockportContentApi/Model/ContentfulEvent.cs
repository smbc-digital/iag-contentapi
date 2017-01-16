using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulEvent
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

        public ContentfulEvent() {}

        public ContentfulEvent(string title, string slug, string teaser, string imageUrl, string description, string fee, 
                     string location, string submittedBy, string longitude, string latitude, bool featured, DateTime eventDate, string startTime, 
                     string endTime, int occurences, EventFrequency frequency, List<Crumb> breadcrumbs) 
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            ImageAsset = new Asset { File = new File { Url = imageUrl } };
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
        }

        private static string ConvertToThumbnail(string thumbnailImage)
        {
            return string.IsNullOrEmpty(thumbnailImage) ? "" : thumbnailImage + "?h=250";
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

        public Event ToModel()
        {
            return new Event
            {
                Title = Title,
                EventDate = EventDate,
                Breadcrumbs = Breadcrumbs,
                Description = Description,
                EndTime = EndTime,
                Featured =  Featured,
                Fee = Fee,
                Frequency = Frequency,
                ImageUrl = ImageUrl,
                Latitude = Latitude,
                Longitude = Longitude,
                Location = Location,
                Occurences = Occurences,
                Slug = Slug,
                StartTime = StartTime,
                SubmittedBy = SubmittedBy,
                Teaser = Teaser,
                ThumbnailImageUrl = ThumbnailImageUrl,
                ImageAsset = ImageAsset,
            };
        }

        public bool IsSameAs(Event otherEvent)
        {
            return 
                Title == otherEvent.Title &&
                Slug == otherEvent.Slug &&
                Teaser == otherEvent.Teaser &&
                ImageAsset == otherEvent.ImageAsset &&
                Description == otherEvent.Description &&
                Fee == otherEvent.Fee &&
                Location == otherEvent.Location &&
                SubmittedBy == otherEvent.SubmittedBy &&
                Longitude == otherEvent.Longitude &&
                Latitude == otherEvent.Latitude &&
                Featured == otherEvent.Featured &&
                EventDate == otherEvent.EventDate &&
                StartTime == otherEvent.StartTime &&
                EndTime == otherEvent.EndTime &&
                Occurences == otherEvent.Occurences &&
                Frequency == otherEvent.Frequency &&
                Breadcrumbs == otherEvent.Breadcrumbs;
        }
    }
}
