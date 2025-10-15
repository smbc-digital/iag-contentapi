namespace StockportContentApiTests.Unit.Builders;

public class AlertBuilder
{
    public Alert Build()
        => new("title",
            "body",
            "severity",
            DateTime.MinValue,
            DateTime.MaxValue,
            "slug",
            false,
            string.Empty,
            new List<string>());
}