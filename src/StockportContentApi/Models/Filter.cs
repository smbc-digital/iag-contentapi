namespace StockportContentApi.Model
{
    public class Filter
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public string Theme { get; set; }
        public bool Highlight { get; set; }

        public Filter(ContentfulFilter contentfulFilter)
        {
            if (contentfulFilter != null)
            {
                Slug = contentfulFilter.Slug;
                Title = contentfulFilter.Title;
                DisplayName = contentfulFilter.DisplayName;
                Theme = contentfulFilter.Theme;
                Highlight = contentfulFilter.Highlight;
            }
        }
    }
}
