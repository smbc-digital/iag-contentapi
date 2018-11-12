﻿using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulKeyFact : IContentfulModel
    {
        public string Icon { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}