﻿using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Linq;
namespace StockportContentApi.Model
{
    public class GroupSearch
    {
        internal string donation { get; set; } = string.Empty;

        public string Category { get; set; }
        public string SubCategories { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Order { get; set; }
        public string Location { get; set; } = "Stockport";
        public string GetInvolved { get; set; } = string.Empty;
        public string Organisation { get; set; } = string.Empty;
    }
}
