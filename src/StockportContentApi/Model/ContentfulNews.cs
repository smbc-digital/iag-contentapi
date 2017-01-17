using System;
using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulNews
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty } };
        public string Body { get; set; } = string.Empty;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public List<Crumb> Breadcrumbs { get; set; } = new List<Crumb> { new Crumb("News", string.Empty, "news") };
        public List<string> Tags { get; set; } = new List<string>();
        public List<Alert> Alerts { get; set; } = new List<Alert>();
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public List<string> Categories { get; set; } = new List<string>();
    }
}