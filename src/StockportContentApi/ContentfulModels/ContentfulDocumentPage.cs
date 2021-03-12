using System;
using System.Collections.Generic;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulDocumentPage : ContentfulReference
    {
        public string AboutThisDocument { get; set; } = string.Empty;
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public string RequestAnAccessibleFormatContactInformation { get; set; } = string.Empty;
        public string FurtherInformation { get; set; } = string.Empty;
        public List<ContentfulReference> RelatedDocuments { get; set; } = new List<ContentfulReference>();
        public DateTime DatePublished { get; set; } = DateTime.MinValue.ToUniversalTime();
        public DateTime LastUpdated { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
    }
}