using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulStartPage
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string UpperBody { get; set; } = string.Empty;
        public string FormLinkLabel { get; set; } = string.Empty;
        public string FormLink { get; set; } = string.Empty;
        public string LowerBody { get; set; } = string.Empty;
        public string BackgroundImage { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    }
}