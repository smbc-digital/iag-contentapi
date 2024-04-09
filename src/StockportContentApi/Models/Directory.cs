namespace StockportContentApi.Model
{
    public class Directory
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ContentfulId { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;
        public string MetaDescription { get; set; }
        public string BackgroundImage { get; set; }
        public string Body { get; set; } = string.Empty;
        public CallToActionBanner CallToAction { get; set; }
        public IEnumerable<Alert> Alerts { get; set; }
        public IEnumerable<DirectoryEntry> Entries { get; set; } = new List<DirectoryEntry>();
        public IEnumerable<Directory> SubDirectories { get; set; } = new List<Directory>();
        public string ColourScheme { get; set; } = string.Empty;
        public string SearchBranding { get; set; } = "Default";
        public string Icon { get; set; } = string.Empty;
        public EventBanner EventBanner { get; set; }
        public IEnumerable<SubItem> RelatedContent { get; set; } = new List<SubItem>();
        public IEnumerable<ExternalLink> ExternalLinks { get; set; }
        public IEnumerable<DirectoryEntry> PinnedEntries { get; set; } = new List<DirectoryEntry>();
    }
}