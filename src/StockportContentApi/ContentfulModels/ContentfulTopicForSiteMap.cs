﻿using System;
using Contentful.Core.Models;
using System.Collections.Generic;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulTopicForSiteMap : IContentfulModel
    {
        public string Slug { get; set; } = string.Empty;
        public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public List<ContentfulSectionForSiteMap> Sections { get; set; } = new List<ContentfulSectionForSiteMap>();
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
