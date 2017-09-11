using System;
using System.Collections.Generic;
using StockportContentApi.Model;

namespace StockportContentApi.ManagementModels
{
    public class ManagementGroup
    {
        public Dictionary<string, string> Name { get; set; }
        public Dictionary<string, string> Slug { get; set; }
        public Dictionary<string, string> PhoneNumber { get; set; }
        public Dictionary<string, string> Email { get; set; }
        public Dictionary<string, string> Website { get; set; }
        public Dictionary<string, string> Twitter { get; set; }
        public Dictionary<string, string> Facebook { get; set; }
        public Dictionary<string, string> Address { get; set; }
        public Dictionary<string, LinkReference> Image { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public Dictionary<string, List<ManagementGroupCategory>> CategoriesReference { get; set; }
        public Dictionary<string, MapPosition> MapPosition { get; set; }
        public Dictionary<string, bool> Volunteering { get; set; }
        public Dictionary<string, GroupAdministrators> GroupAdministrators { get; set; }
        public Dictionary<string, string> DateHiddenFrom { get; set; }
        public Dictionary<string, string> DateHiddenTo { get; set; }
        public Dictionary<string, List<string>> Cost { get; set; }
        public Dictionary<string, string> CostText { get; set; }
        public Dictionary<string, string> AbilityLevel { get; set; }
        public Dictionary<string, string> VolunteeringText { get; set; }
        public Dictionary<string, ManagementReference> Organisation { get; set; }
        public Dictionary<string, List<ManagementReference>> SubCategories { get; set; }
    }
}
