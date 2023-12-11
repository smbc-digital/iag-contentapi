namespace StockportContentApi.Models
{
    public class FilterTheme
    {
        public string Title { get; set; }

        public IEnumerable<Filter> Filters { get; set; }
    }
}
