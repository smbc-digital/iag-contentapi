using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulEvent
    {      
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;      
        public Asset Image { get; set; } = new Asset { File = new File { Url = "" }, SystemProperties = new SystemProperties { Type = "Asset" } };             
        public string Description { get; set; } = string.Empty;
        public string Fee { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public DateTime EventDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int Occurences { get; set; } = 0;
        public EventFrequency Frequency { get; set; } = EventFrequency.None;
        public List<Crumb> Breadcrumbs { get; set; } = new List<Crumb> { new Crumb("Events", string.Empty, "events") };
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public List<string> Categories { get; set; } = new List<string>();
        public MapPosition MapPosition { get; set; } = new MapPosition();
        public string BookingInformation { get; set; } = string.Empty;
        public bool Featured { get; set; } = false;
        public ContentfulEventSys Sys { get; set; } = new ContentfulEventSys();
        public List<string> Tags { get; set; } = new List<string>();
        public List<Entry<ContentfulAlert>> Alerts { get; set; } = new List<Entry<ContentfulAlert>>();

        public Entry<ContentfulGroup> Group { get; set; } = new Entry<ContentfulGroup>
        {
            Fields = new ContentfulGroup(),
            SystemProperties = new SystemProperties {Type = "Entry"}
        };
    }

    public class ContentfulEventSys
    {
        public DateTime UpdatedAt { get; set; } = DateTime.MinValue.ToUniversalTime();
    }
}
