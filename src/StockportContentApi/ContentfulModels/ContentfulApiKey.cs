using System;
using System.Collections.Generic;

namespace StockportContentApi.ContentfulModels
{
    
    public class ContentfulApiKey
    {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int Version { get; set; } = 0;
        public string Email { get; set; } = string.Empty;
        public DateTime ActiveFrom { get; set; } = DateTime.MinValue;
        public DateTime ActiveTo { get; set; } = DateTime.MaxValue;
        public List<string> EndPoints { get; set; } = new List<string>();
        public List<string> AllowedVerbs{ get; set; } = new List<string>();
        public bool CanViewSensitive { get; set; } = false;
    }
}