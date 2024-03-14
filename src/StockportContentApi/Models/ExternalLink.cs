namespace StockportContentApi.Model
{
    public class ExternalLink
    {
        public string Title { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Teaser { get; set; } = string.Empty;

        public ExternalLink(string title, string url, string teaser)
        {
            Title = title;
            URL = url;
            Teaser = teaser;
        }
    }
}