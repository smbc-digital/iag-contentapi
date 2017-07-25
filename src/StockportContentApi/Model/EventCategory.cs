namespace StockportContentApi.Model
{
    public class EventCategory
    {
        public string Name { get; }
        public string Slug { get; }
        public string Icon { get; }

        public EventCategory(string name, string slug, string icon)
        {
            Name = name;
            Slug = slug;
            Icon = icon;
        }
    }
}


