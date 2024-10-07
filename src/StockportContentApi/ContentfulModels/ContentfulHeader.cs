namespace StockportContentApi.ContentfulModels
{
    public class ContentfulHeader
    {
        public string Title { get; set; } = string.Empty;
        
        public List<ContentfulReference> Items { get; set; } = new();

        public Asset Logo { get; set; } = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    }
}