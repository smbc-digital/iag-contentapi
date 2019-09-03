﻿using Contentful.Core.Models;
using System.Collections.Generic;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulCommsHomepage : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;

        public ContentfulCallToActionBanner CallToActionBanner { get; set; } = null;

        public string LatestNewsHeader { get; set; } = string.Empty;

        public string TwitterFeedHeader { get; set; } = string.Empty;

        public string InstagramFeedTitle { get; set; } = string.Empty;

        public string InstagramLink { get; set; } = string.Empty;

        public string FacebookFeedTitle { get; set; } = string.Empty;

        public List<ContentfulBasicLink> UsefullLinks { get; set; } = null;

        public ContentfulEvent WhatsOnInStockportEvent { get; set; } = null;

        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
