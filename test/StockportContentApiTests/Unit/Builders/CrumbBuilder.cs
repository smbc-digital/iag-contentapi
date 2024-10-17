namespace StockportContentApiTests.Unit.Builders;

public class CrumbBuilder
{
    private readonly string _slug = "slug";
    private readonly string _title = "title";
    private readonly string _name = "name";

    public Crumb Build()
        => new(_title, _slug, _name);
}