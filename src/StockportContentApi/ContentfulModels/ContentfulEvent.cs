using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulEvent : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public string MetaDescription { get; set; } = string.Empty;
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
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public List<string> Categories { get; set; } = new List<string>();
        public MapPosition MapPosition { get; set; } = new MapPosition();
        public string BookingInformation { get; set; } = string.Empty;
        public bool Featured { get; set; } = false;
        public SystemProperties Sys { get; set; } = new SystemProperties();
        public List<string> Tags { get; set; } = new List<string>();
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public ContentfulGroup  Group { get; set; } = new ContentfulGroup();
        public List<ContentfulEventCategory> EventCategories { get; set; } = new List<ContentfulEventCategory>();
        public bool? Free { get; set; } = null;
        public bool? Paid { get; set; } = null;
        public string AccessibleTransportLink { get; set; } = "/accessibleTransport";
    }
}
