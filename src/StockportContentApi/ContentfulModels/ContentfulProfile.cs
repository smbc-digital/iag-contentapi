using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulProfile
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty } };
        public string Body { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public Asset BackgroundImage { get; set; } = new Asset {File = new File {Url = string.Empty}};
        public List<Entry<ContentfulCrumb>> Breadcrumbs { get; set; } = new List<Entry<ContentfulCrumb>>();
    }
}
