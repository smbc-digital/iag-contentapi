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
            Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
            Body = string.Empty,
            Breadcrumbs = new List<ContentfulReference>()
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
