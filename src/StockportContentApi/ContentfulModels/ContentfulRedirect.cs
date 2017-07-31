using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulRedirect
    {
        public string Title { get; set; } = string.Empty;
        public Dictionary<string, string> Redirects { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> LegacyUrls { get; set; } = new Dictionary<string, string>();
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
