using Contentful.Core.Models;

namespace StockportContentApi.ContentfulModels
{

    public class ContentfulArticle : ContentfulReference
    {
        public string Body { get; set; } = string.Empty;
        public Asset BackgroundImage { get; set; } = new Asset
        {
            File = new File { Url = string.Empty },
            SystemProperties = new SystemProperties { Type = "Asset" }
        };

        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public List<ContentfulProfile> Profiles { get; set; } = new List<ContentfulProfile>();
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public List<ContentfulAlert> AlertsInline { get; set; } = new List<ContentfulAlert>();

        // references
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
    }
}