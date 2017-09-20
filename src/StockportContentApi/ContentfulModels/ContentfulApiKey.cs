using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    
    public class ContentfulApiKey
    {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime ActiveFrom { get; set; } = DateTime.MinValue;
        public DateTime ActiveTo { get; set; } = DateTime.MaxValue;
        public List<string> EndPoints { get; set; } = new List<string>();
    }
}