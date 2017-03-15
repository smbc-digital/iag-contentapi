using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.ContentfulModels
{
    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class ContentfulPayment
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PaymentDetailsText { get; set; } = string.Empty;
        public string ReferenceLabel { get; set; } = string.Empty;
        public string ParisReference { get; set; } = string.Empty;
        public string Fund { get; set; } = string.Empty;
        public string GlCodeCostCentreNumber { get; set; } = string.Empty;
        public List<Entry<ContentfulCrumb>> Breadcrumbs { get; set; } = new List<Entry<ContentfulCrumb>>();
    }
}
