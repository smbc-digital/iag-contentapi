namespace StockportContentApiTests.Unit.Builders;

class AlertBuilder
{
    private readonly string _title = "title";
    private readonly string _body = "body";
    private readonly string _severity = "severity";
    private readonly DateTime _sunriseDate = new(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private readonly DateTime _sunsetDate = new(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
    private readonly string _slug = "slug";
    private readonly bool _isStatic = false;
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };

    public Alert Build()
        => new(_title,
                _body,
                _severity,
                _sunriseDate,
                _sunsetDate,
                _slug,
                _isStatic,
                string.Empty);
}