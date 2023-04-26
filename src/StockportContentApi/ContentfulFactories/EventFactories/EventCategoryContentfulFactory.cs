namespace StockportContentApi.ContentfulFactories.EventFactories;

public class EventCategoryContentfulFactory : IContentfulFactory<ContentfulEventCategory, EventCategory>
{
    public EventCategory ToModel(ContentfulEventCategory entry)
    {
        var name = !string.IsNullOrEmpty(entry.Name)
            ? entry.Name
            : "";

        var slug = !string.IsNullOrEmpty(entry.Slug)
            ? entry.Slug
            : "";

        var icon = !string.IsNullOrEmpty(entry.Icon)
            ? entry.Icon
            : "";

        return new EventCategory(name, slug, icon);
    }
}