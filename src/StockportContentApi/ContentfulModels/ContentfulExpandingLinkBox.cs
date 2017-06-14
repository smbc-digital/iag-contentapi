using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulExpandingLinkBox
    {
        public string Title { get; set; } = string.Empty;
        public List<Entry<ContentfulSubItem>> Links { get; set; } = new List<Entry<ContentfulSubItem>> ();
    }
}
