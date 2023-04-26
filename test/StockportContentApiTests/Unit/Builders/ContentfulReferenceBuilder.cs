namespace StockportContentApiTests.Unit.Builders;

public class ContentfulReferenceBuilder
{
    private string _slug = "slug";
    private string _title = "title";
    private string _name = "name";
    private string _teaser = "teaser";
    private string _icon = "icon";
    private DateTime _sunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private DateTime _sunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
    private Asset _image = new Asset { File = new File { Url = "image" }, SystemProperties = new SystemProperties { Type = "Asset" } };
    private List<ContentfulReference> _subItems = new List<ContentfulReference>();
    private List<ContentfulReference> _secondaryItems = new List<ContentfulReference>();
    private List<ContentfulReference> _tertiaryItems = new List<ContentfulReference>();
    private string _systemId = "id";
    private string _contentTypeSystemId = "id";

    public ContentfulReference Build()
    {
        return new ContentfulReference
        {
            Slug = _slug,
            Title = _title,
            Name = _name,
            Teaser = _teaser,
            Icon = _icon,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Image = _image,
            SubItems = _subItems,
            SecondaryItems = _secondaryItems,
            TertiaryItems = _tertiaryItems,
            Sys = new SystemProperties
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                Id = _systemId
            }
        };
    }

    public ContentfulReferenceBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulReferenceBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulReferenceBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public ContentfulReferenceBuilder SubItems(List<ContentfulReference> subItems)
    {
        _subItems = subItems;
        return this;
    }

    public ContentfulReferenceBuilder SecondaryItems(List<ContentfulReference> secondaryItems)
    {
        _secondaryItems = secondaryItems;
        return this;
    }

    public ContentfulReferenceBuilder TertiaryItems(List<ContentfulReference> tertiaryItems)
    {
        _tertiaryItems = tertiaryItems;
        return this;
    }

    public ContentfulReferenceBuilder SystemId(string id)
    {
        _systemId = id;
        return this;
    }

    public ContentfulReferenceBuilder SystemContentTypeId(string id)
    {
        _contentTypeSystemId = id;
        return this;
    }

    public ContentfulReferenceBuilder Icon(string icon)
    {
        _icon = icon;
        return this;
    }
}
