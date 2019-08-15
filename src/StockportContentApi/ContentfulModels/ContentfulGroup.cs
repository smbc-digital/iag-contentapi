using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulGroup : IContentfulModel
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Twitter { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Asset Image { get; set; } = new Asset { File = new File { Url = "" }, SystemProperties = new SystemProperties { Type = "Asset" } };
        public List<ContentfulGroupCategory> CategoriesReference { get; set; } = new List<ContentfulGroupCategory>();
        public List<ContentfulGroupSubCategory> SubCategories { get; set; } = new List<ContentfulGroupSubCategory>();
        public MapPosition  MapPosition { get; set; } = new MapPosition();
        public bool Volunteering { get; set; } = false;
        public bool Donations { get; set; } = false;
        public SystemProperties Sys { get; set; } = new SystemProperties();
        public GroupAdministrators GroupAdministrators { get; set; } = new GroupAdministrators();
        public DateTime? DateHiddenFrom { get; set; }
        public DateTime? DateHiddenTo { get; set; }
        public List<string> Cost { get; set; } = new List<string>();
        public string CostText { get; set; }
        public string AbilityLevel { get; set; }
        public string VolunteeringText { get; set; }
        public ContentfulOrganisation Organisation { get; set; } = new ContentfulOrganisation();
        public string AccessibleTransportLink { get; set; } = "/accessibleTransport";
        public string AdditionalInformation { get; set; } = string.Empty;
        public List<Asset> AdditionalDocuments { get; set; } = new List<Asset>();
        public List<string> SuitableFor { get; set; } = new List<string>();
        public List<string> AgeRange { get; set; } = new List<string>();
        public string DonationsText { get; set; }
        public string DonationsUrl { get; set; }
    }
}
