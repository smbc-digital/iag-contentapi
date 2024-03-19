namespace StockportContentApiTests.Unit.Builders;

public class SubItemBuilder
{
    private readonly string _slug = "slug";
    private readonly string _title = "title";
    private readonly string _teaser = "teaser";
    private readonly string _icon = "icon";
    private readonly string _type = "type";
    private readonly DateTime _sunriseDate = DateTime.MinValue;
    private readonly DateTime _sunsetDate = DateTime.MaxValue;
    private readonly string _image = "image";
    private readonly List<SubItem> _subItems = new List<SubItem>();

    public SubItem Build()
    {
        return new SubItem(_slug, _title, _teaser, _icon, _type, _sunriseDate, _sunsetDate, _image, _subItems);
    }
}