namespace StockportContentApi.ContentfulModels
{
    public class ContentfulDirectoryEntry : ContentfulReference
    {
        public string Body { get; set; }
        public IEnumerable<ContentfulFilter> Filters { get; set; }
        public IEnumerable<ContentfulDirectory> Directories { get; set; }
        public MapPosition MapPosition { get; set; } = new MapPosition();
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Twitter { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
