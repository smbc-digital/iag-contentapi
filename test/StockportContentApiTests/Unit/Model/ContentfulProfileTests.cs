namespace StockportContentApiTests.Unit.Model;

public class ContentfulProfileTests
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        ContentfulProfile actual = new();
        ContentfulProfile expected = new()
        {
            Title = string.Empty,
            Slug = string.Empty,
            Subtitle = string.Empty,
            Body = string.Empty,
            Breadcrumbs = new List<ContentfulReference>()
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
