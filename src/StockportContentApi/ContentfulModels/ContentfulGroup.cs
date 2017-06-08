using System.Collections.Generic;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulGroup
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Twitter { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Asset Image { get; set; } = new Asset { File = new File { Url = "" }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public List<ContentfulGroupCategory> CategoriesReference { get; set; } = new List<ContentfulGroupCategory>();
        public List<Crumb> Breadcrumbs { get; set; } = new List<Crumb> { new Crumb("Find a local group", string.Empty, "groups") };
        public MapPosition  MapPosition = new MapPosition();
        public bool Volunteering = false ;
        public SystemProperties Sys = new SystemProperties();
    }
}
