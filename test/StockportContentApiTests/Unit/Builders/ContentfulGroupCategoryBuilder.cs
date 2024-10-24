namespace StockportContentApiTests.Builders;

public class ContentfulGroupCategoryBuilder
{
    private string _name { get; set; } = "name";
    private string _slug { get; set; } = "slug";
    private string _icon { get; set; } = "icon";
    private Asset _image { get; set; } = new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };

    public ContentfulGroupCategory Build()
        => new()
        {
            Name = _name,
            Slug = _slug,
            Icon = _icon,
            Image = _image,
            Sys = _sys
        };

    public ContentfulGroupCategoryBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulGroupCategoryBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public ContentfulGroupCategoryBuilder Image(Asset Image)
    {
        _image = Image;
        return this;
    }

    public ContentfulGroupCategoryBuilder Icon(string icon)
    {
        _icon = icon;
        return this;
    }
}