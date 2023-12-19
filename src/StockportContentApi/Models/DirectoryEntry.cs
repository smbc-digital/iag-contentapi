namespace StockportContentApi.Model
{
    public class DirectoryEntry
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Teaser { get; set; }
        public string MetaDescription { get; set; }
        public IEnumerable<FilterTheme> Themes { get; set; } = new List<FilterTheme>();
        public IEnumerable<MinimalDirectory> Directories { get; set; }
        public IEnumerable<Alert> Alerts { get; set; }
        public IEnumerable<GroupBranding> Branding { get; set; } = new List<GroupBranding>();
        public MapPosition MapPosition { get; set; } = new MapPosition();
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Twitter { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;        
        public readonly record struct MinimalDirectory(string Slug, string Title);

    }
}
