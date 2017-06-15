using System;
using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulConsultation : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public DateTime ClosingDate { get; set; } = DateTime.Now.AddDays(-1);
        public string Link { get; set; } = string.Empty;
        public SystemProperties Sys { get; set; } = new SystemProperties();
    }
}
