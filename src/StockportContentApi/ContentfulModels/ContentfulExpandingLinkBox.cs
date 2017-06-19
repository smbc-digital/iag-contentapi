using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulExpandingLinkBox : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public List<ContentfulReference> Links { get; set; } = new List<ContentfulReference> ();
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
