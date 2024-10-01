namespace StockportContentApiTests.Builders;

public class ContentfulEventCategoryBuilder
{
    private string _name { get; set; } = "name";
    private string _slug { get; set; } = "slug";
    private string _icon { get; set; } = "icon";
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };

    public ContentfulEventCategory Build()
    {
        return new ContentfulEventCategory()
        {
            Name = _name,
            Slug = _slug,
            Icon = _icon,
            Sys = _sys
        };
    }

    public ContentfulEventCategoryBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulEventCategoryBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public ContentfulEventCategoryBuilder Icon(string icon)
    {
        _icon = icon;
        return this;
    }
}