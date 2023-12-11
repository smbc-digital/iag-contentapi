using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulFilter : ContentfulReference
    {
        public string DisplayName { get; set; }
        public string Theme { get; set; }
    }
}
