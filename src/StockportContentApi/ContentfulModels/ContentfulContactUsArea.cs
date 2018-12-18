using System.Collections.Generic;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulContactUsArea : ContentfulReference
    {
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public List<ContentfulReference> PrimaryItems { get; set; } = new List<ContentfulReference>();
    }
}