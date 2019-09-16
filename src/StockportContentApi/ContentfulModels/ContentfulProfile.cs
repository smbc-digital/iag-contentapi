using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulProfile : IContentfulModel
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Quote { get; set; } = string.Empty;
        public List<ContentfulInlineQuote> InlineQuotes { get; set; } = new List<ContentfulInlineQuote>(); 
        public Asset Image { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public string Body { get; set; } = string.Empty;
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public SystemProperties Sys { get; set; } = new SystemProperties();
        public string TriviaSubheading { get; set; }
        public List<ContentfulInformationList> TriviaSection { get; set; } = new List<ContentfulInformationList>();
        public FieldOrder FieldOrder { get; set; } = new FieldOrder();
        public string Author { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public ContentfulButton Button { get; set; } = new ContentfulButton();
        public ContentfulEventBanner EventsBanner {get; set; } = new ContentfulEventBanner();
    }
}
