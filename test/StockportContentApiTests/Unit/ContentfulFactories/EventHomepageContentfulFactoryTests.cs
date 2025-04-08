namespace StockportContentApiTests.Unit.ContentfulFactories;

public class EventHomepageContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly EventHomepageContentfulFactory _factory;

    private readonly ContentfulEventHomepage _entry = new()
    {
        TagOrCategory1 = "TagOrCategory1",
        TagOrCategory2 = "TagOrCategory2",
        TagOrCategory3 = "TagOrCategory3",
        TagOrCategory4 = "TagOrCategory4",
        TagOrCategory5 = "TagOrCategory5",
        TagOrCategory6 = "TagOrCategory6",
        TagOrCategory7 = "TagOrCategory7",
        TagOrCategory8 = "TagOrCategory8",
        TagOrCategory9 = "TagOrCategory9",
        TagOrCategory10 = "TagOrCategory10",
        MetaDescription = "MetaDescription",
        Alerts = new List<ContentfulAlert> { new() },
        GlobalAlerts = new List<ContentfulAlert> { new() }
    };

    public EventHomepageContentfulFactoryTests()
    {
        _timeProvider
            .Setup(time => time.Now())
            .Returns(new DateTime(2017, 01, 01));

        _factory = new(_callToActionFactory.Object, _alertFactory.Object, _timeProvider.Object);
    }

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

    [Fact]
    public void ToModel_ShouldCallAlertFactory_If_CallToActionNotNull()
    {
        // Arrange
        _alertFactory
            .Setup(alertFactory => alertFactory.ToModel(It.IsAny<ContentfulAlert>()))
            .Returns(new Alert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()));

        // Act
        EventHomepage result = _factory.ToModel(_entry);

        // Assert
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(It.IsAny<ContentfulAlert>()), Times.Exactly(2));
    }
}