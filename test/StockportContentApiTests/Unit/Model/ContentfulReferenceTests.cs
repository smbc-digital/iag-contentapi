namespace StockportContentApiTests.Unit.Model;

public class ContentfulReferenceTests
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        ContentfulReference actual = new();
        ContentfulReference expected = new()
        {
            Slug = string.Empty,
            Title = string.Empty,
            Name = string.Empty,
            Teaser = string.Empty,
            Icon = string.Empty,
            SunriseDate = DateTime.MinValue.ToUniversalTime(),
            SunsetDate = DateTime.MaxValue.ToUniversalTime()
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
