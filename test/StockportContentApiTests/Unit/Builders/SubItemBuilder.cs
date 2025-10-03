namespace StockportContentApiTests.Unit.Builders;

public class SubItemBuilder
{
    private readonly List<SubItem> _subItems = new();

    public SubItem Build()
        => new("slug",
            "title",
            "teaser",
            "teaserImage",
            "icon",
            "type",
            DateTime.MinValue,
            DateTime.MaxValue,
            "image",
            _subItems,
            EColourScheme.Blue);
}