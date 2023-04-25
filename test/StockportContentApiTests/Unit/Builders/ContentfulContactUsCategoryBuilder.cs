namespace StockportContentApiTests.Unit.Builders;

public class ContentfulContactUsCategoryBuilder
{
    private string _title = "title";
    private string _bodyTextLeft = "body";
    private string _bodyTextRight = "body";
    private string _icon = "icon";

    private SystemProperties _sys = new SystemProperties
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };

    public ContentfulContactUsCategory Build()
    {
        return new ContentfulContactUsCategory
        {
            Title = _title,
            BodyTextLeft = _bodyTextLeft,
            BodyTextRight = _bodyTextRight,
            Sys = _sys,
            Icon = _icon
        };
    }

    public ContentfulContactUsCategoryBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulContactUsCategoryBuilder BodyTextLeft(string bodyTextLeft)
    {
        _bodyTextLeft = bodyTextLeft;
        return this;
    }

    public ContentfulContactUsCategoryBuilder BodyTextRight(string bodyTextRight)
    {
        _bodyTextRight = bodyTextRight;
        return this;
    }

    public ContentfulContactUsCategoryBuilder Icon(string icon)
    {
        _icon = icon;
        return this;
    }

}
