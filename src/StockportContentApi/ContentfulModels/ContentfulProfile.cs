﻿using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulProfile : IContentfulModel
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string LeadParagraph { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public string Body { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public Asset BackgroundImage { get; set; } = new Asset {File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public List<ContentfulAlert> AlertsInline { get; set; } = new List<ContentfulAlert>();
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
