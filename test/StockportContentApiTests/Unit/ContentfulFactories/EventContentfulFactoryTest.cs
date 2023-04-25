namespace StockportContentApiTests.Unit.ContentfulFactories;

public class EventContentfulFactoryTest
{
    private readonly ContentfulEvent _contentfulEvent;
    private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private readonly Mock<ITimeProvider> _timeProvider;
    private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
    private readonly Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>> _eventCategoryFactory;
    private readonly EventContentfulFactory _eventContentfulFactory;
    private readonly List<Alert> _alerts = new List<Alert>() {
            new Alert("title", "subHeading", "body", "severity", new DateTime(0001, 1, 1), new DateTime(9999, 9, 9), string.Empty, false) };

    public EventContentfulFactoryTest()
    {
        _contentfulEvent = new ContentfulEventBuilder().Build();

        _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
        _eventCategoryFactory = new Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>>();
        _timeProvider = new Mock<ITimeProvider>();

        _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

        _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "subHeading", "body",
                                                             "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                                             new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc), "slug", false));

        _eventContentfulFactory = new EventContentfulFactory(_documentFactory.Object, _groupFactory.Object, _eventCategoryFactory.Object, _alertFactory.Object, _timeProvider.Object);

    }

    [Fact]
    public void ShouldNotAddDocumentsOrImageIfTheyAreLinks()
    {
        _contentfulEvent.Documents.First().SystemProperties.LinkType = "Link";
        _contentfulEvent.Image.SystemProperties.LinkType = "Link";

        var anEvent = _eventContentfulFactory.ToModel(_contentfulEvent);

        anEvent.Documents.Count.Should().Be(0);
        anEvent.ImageUrl.Should().Be(string.Empty);
        anEvent.ThumbnailImageUrl.Should().Be(string.Empty);
    }

    [Fact]
    public void ShouldReturnGroupLinkedToEvent()
    {
        _groupFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroup>()))
            .Returns(new GroupBuilder().Name("Test Group").Build());

        var anEvent = _eventContentfulFactory.ToModel(_contentfulEvent);

        anEvent.Group.Name.Should().Be("Test Group");
    }
}
