namespace StockportContentApi.ContentfulFactories.EventFactories;

public class EventCategoryContentfulFactory : IContentfulFactory<ContentfulEventCategory, EventCategory>
{
    public EventCategory ToModel(ContentfulEventCategory entry)
    {
        string name = !string.IsNullOrEmpty(entry.Name)
            ? entry.Name
            : string.Empty;

        string slug = !string.IsNullOrEmpty(entry.Slug)
            ? entry.Slug
            : string.Empty;

        string icon = !string.IsNullOrEmpty(entry.Icon)
            ? entry.Icon
            : string.Empty;

        return new EventCategory(name, slug, icon);
    }
}