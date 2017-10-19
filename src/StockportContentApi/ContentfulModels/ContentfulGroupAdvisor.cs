using System.Collections.Generic;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulGroupAdvisor
    {
        public string Name { get; set; } = "";
        public string EmailAddress { get; set; } = "";
        public IEnumerable<ContentfulReference> Groups { get; set; } = new List<ContentfulReference>();
        public bool HasGlobalAccess { get; set; } = false;
    }
}
