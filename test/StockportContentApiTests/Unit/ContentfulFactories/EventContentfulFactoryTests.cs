namespace StockportContentApiTests.Unit.ContentfulFactories;

public class EventContentfulFactoryTests
{
    private readonly ContentfulEvent _contentfulEvent;
    private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>> _eventCategoryFactory = new();
    private readonly EventContentfulFactory _eventContentfulFactory;

    public EventContentfulFactoryTests()
    {
        _contentfulEvent = new ContentfulEventBuilder().Build();

        _timeProvider
            .Setup(time => time.Now())
            .Returns(new DateTime(2017, 01, 01));

        _alertFactory
            .Setup(alert => alert.ToModel(It.IsAny<ContentfulAlert>()))
            .Returns(new Alert("title",
                            "subHeading",
                            "body",
                            "severity",
                            new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                            new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc),
                            "slug",
                            false,
                            string.Empty));

        _eventContentfulFactory = new(_documentFactory.Object,
                                    _groupFactory.Object,
                                    _eventCategoryFactory.Object,
                                    _alertFactory.Object,
                                    _timeProvider.Object);
    }

    [Fact]
    public void ToModel_ShouldNotAddDocumentsOrImage_If_TheyAreLinks()
    {
        // Arrange
        _contentfulEvent.Documents.First().SystemProperties.LinkType = "Link";
        _contentfulEvent.Image.SystemProperties.LinkType = "Link";

        // Act
        Event result = _eventContentfulFactory.ToModel(_contentfulEvent);

        // Assert
        Assert.Empty(result.Documents);
        Assert.Empty(result.ImageUrl);
        Assert.Empty(result.ThumbnailImageUrl);
    }

    [Fact]
    public void ToModel_ShouldReturnGroupLinkedToEvent()
    {
        // Arrange
        _groupFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulGroup>()))
            .Returns(new GroupBuilder().Name("Test Group").Build());

        // Act
        Event result = _eventContentfulFactory.ToModel(_contentfulEvent);

        // Assert
        Assert.Equal("Test Group", result.Group.Name);
    }
}