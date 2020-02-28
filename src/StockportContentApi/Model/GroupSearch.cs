﻿namespace StockportContentApi.Model
{
    public class GroupSearch
    {
        public string Category { get; set; }
        public string SubCategories { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Order { get; set; }
        public string Location { get; set; } = "Stockport";
        public string GetInvolved { get; set; } = string.Empty;
        public string Organisation { get; set; } = string.Empty;

        // Used for filtering contentful tags field
        public string Tags { get; set; } = string.Empty;

        // Used for filtering contentful organisation field
        public string Tag { get; set; } = string.Empty;
    }
}
