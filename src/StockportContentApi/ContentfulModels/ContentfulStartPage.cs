using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulStartPage : ContentfulReference
    {
        public string Summary { get; set; } = string.Empty;
        public string UpperBody { get; set; } = string.Empty;
        public string FormLinkLabel { get; set; } = string.Empty;
        public string FormLink { get; set; } = string.Empty;
        public string LowerBody { get; set; } = string.Empty;
        public string BackgroundImage { get; set; } = string.Empty;
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    }
}