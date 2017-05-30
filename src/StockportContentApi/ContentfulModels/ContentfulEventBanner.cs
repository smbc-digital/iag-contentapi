using Contentful.Core.Configuration;
using Newtonsoft.Json;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulEventBanner
    {
        public string Title { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}