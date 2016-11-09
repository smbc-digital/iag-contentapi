namespace StockportContentApi.Model
{
    public class Crumb
    {     
        public string Title { get; }
        public string Slug { get; }
        public string Type { get; }

        public Crumb(string title, string slug, string type)
        {
            Title = title;
            Slug = slug;
            Type = type;
        }
    }

    public class NullCrumb : Crumb
    {
        public NullCrumb():base(string.Empty, string.Empty, string.Empty) {}
    }
}
