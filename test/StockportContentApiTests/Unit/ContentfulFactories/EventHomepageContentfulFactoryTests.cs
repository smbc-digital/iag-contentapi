namespace StockportContentApiTests.Unit.ContentfulFactories;

public class EventHomepageContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();
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

    public EventHomepageContentfulFactoryTests() =>
        _factory = new(_callToActionFactory.Object);

    [Fact]
    public void ToModel_ShouldReturnExpectedEventHomepage()
    {
        // Act
        EventHomepage result = _factory.ToModel(_entry);

        // Assert
        Assert.Equal(11, result.Rows.Count());
    }

    [Fact]
    public void ToModel_ShouldCallCallToActionFactory_If_CallToActionNotNull()
    {
        // Arrange
        _callToActionFactory
            .Setup(callToActionFactory => callToActionFactory.ToModel(It.IsAny<ContentfulCallToActionBanner>()))
            .Returns(new CallToActionBanner());

        // Act
        EventHomepage result = _factory.ToModel(_entry);

        // Assert
        _callToActionFactory.Verify(callToAction => callToAction.ToModel(It.IsAny<ContentfulCallToActionBanner>()), Times.Once);

    }
}