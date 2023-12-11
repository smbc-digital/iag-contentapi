using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulDirectoryEntry : ContentfulReference
    {
        public string Body { get; set; }

        public IEnumerable<ContentfulFilter> Filters { get; set; }

        public IEnumerable<ContentfulDirectory> Directories { get; set; }
    }
}
