namespace StockportContentApiTests.Unit.Model;

public class ContentfulProfileTest
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        var actual = new ContentfulProfile();
        var expected = new ContentfulProfile
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
