﻿using System.Collections.Generic;
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
    }
}