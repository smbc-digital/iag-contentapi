namespace StockportContentApiTests.Unit.Builders;

public class ContentfulEventCategoryBuilder
{
    private string _name = "name";
    private string _slug = "slug";
    private string _icon = "icon";
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };

    public ContentfulEventCategory Build()
        => new()
        {
            Name = _name,
            Slug = _slug,
            Icon = _icon,
            Sys = _sys
        };

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