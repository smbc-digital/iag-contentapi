namespace StockportContentApiTests.Unit.Model;

public class ContentfulSectionTest
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        var actual = new ContentfulSection();
        var expected = new ContentfulSection
        {
            Title = string.Empty,
            Slug = string.Empty,
            Body = string.Empty,
            Profiles = new List<ContentfulProfile>(),
            Documents = new List<Asset>(),
            SunriseDate = DateTime.MinValue.ToUniversalTime(),
            SunsetDate = DateTime.MaxValue.ToUniversalTime()
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
