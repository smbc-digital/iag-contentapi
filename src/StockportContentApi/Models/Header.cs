namespace StockportContentApi.Models
{
    public class Header
    {
        public Header(string title, List<SubItem> items, string logo)
        {
            Title = title;
            Items = items;
            Logo  = logo;
        }

        public string Title { get; set; } = string.Empty;

        public List<SubItem> Items { get; set; } = new();

        public string Logo { get; set; }
    }
}