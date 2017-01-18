using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using StockportContentApi.Utils;

namespace StockportContentApi.Model
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulEvent
    {      
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;      
        public Asset Image { get; set; } = new Asset { File = new File { Url = "" } };             
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
        public List<Asset> Documents { get; set; } = new List<Asset>();

        public Event ToModel()
        {
            var eventDocuments = Documents.Select(
                document => 
                new Document(document.Description, 
                             (int)document.File.Details.Size, 
                             DateComparer.DateFieldToDate(document.SystemProperties.UpdatedAt), 
                             document.File.Url, document.File.FileName)).ToList();

            return new Event(Title, Slug, Teaser, Image.File.Url, Description, Fee, Location, SubmittedBy, Longitude,
                Latitude, Featured, EventDate, StartTime, EndTime, Occurences, Frequency, Breadcrumbs,
                ConvertToThumbnail(Image.File.Url), eventDocuments);
        }

        private static string ConvertToThumbnail(string thumbnailImage)
        {
            return string.IsNullOrEmpty(thumbnailImage) ? "" : thumbnailImage + "?h=250";
        }
    }
}
