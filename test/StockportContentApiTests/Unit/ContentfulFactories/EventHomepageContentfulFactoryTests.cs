namespace StockportContentApiTests.Unit.ContentfulFactories;

public class EventHomepageContentfulFactoryTests
{
    private readonly Mock<ITimeProvider> _mockTimeProvider = new();

    private readonly EventHomepageContentfulFactory _factory;

    private readonly ContentfulEventHomepage _entry = new()
    {
        Tag1 = "Tag1",
        Tag2 = "Tag2",
        Tag3 = "Tag3",
        Tag4 = "Tag4",
        Tag5 = "Tag5",
        Tag6 = "Tag6",
        Tag7 = "Tag7",
        Tag8 = "Tag8",
        Tag9 = "Tag9",
        Tag10 = "Tag10",
        MetaDescription = "MetaDescription",
        Alerts = new List<ContentfulAlert> { new() }
    };

    public EventHomepageContentfulFactoryTests()
    {
        _factory = new EventHomepageContentfulFactory(_mockTimeProvider.Object);
    }

    [Fact]
    public void ToModel_ShouldReturnExpectedEventHomepage()
    {
        // Act
        var result = _factory.ToModel(_entry);

        // Assert
        Assert.Equal(11, result.Rows.Count());
    }
}
