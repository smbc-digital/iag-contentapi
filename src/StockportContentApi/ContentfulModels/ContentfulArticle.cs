using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

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
        public List<ContentfulSection> Sections { get; set; } = new List<ContentfulSection>();
        
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public List<ContentfulProfile> Profiles { get; set; } = new List<ContentfulProfile>();
        public List<Asset> Documents { get; set; } = new List<Asset>();
        public List<ContentfulAlert> AlertsInline { get; set; } = new List<ContentfulAlert>();
      
        public bool LiveChatVisible { get; set; } = false;
        public ContentfulLiveChat LiveChatText { get; set; } = new ContentfulLiveChat();
       
        // references
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public ContentfulAdvertisement Advertisement { get; set;  } = new ContentfulAdvertisement();
    }
}