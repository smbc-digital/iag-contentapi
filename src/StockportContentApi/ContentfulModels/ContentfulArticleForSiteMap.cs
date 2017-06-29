using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulArticleForSiteMap : IContentfulModel
    {
        public string Slug { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
