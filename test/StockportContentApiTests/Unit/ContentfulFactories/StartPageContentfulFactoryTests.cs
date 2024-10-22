namespace StockportContentApiTests.Unit.ContentfulFactories;

public class StartPageContentfulFactoryTests
{
    private readonly StartPageContentfulFactory _factory;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();

    public StartPageContentfulFactoryTests()
    {
        _alertFactory
            .Setup(alertFactory => alertFactory.ToModel(It.IsAny<ContentfulAlert>()))
            .Returns(new Alert("title", string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue, string.Empty, false, string.Empty));
        
        _factory = new StartPageContentfulFactory(_timeProvider.Object, _alertFactory.Object, _crumbFactory.Object);
    }

    [Fact]
    public void ToModel_ShouldBuildStartPageFromContentfulStartPage()
    {
        // Arrange
        ContentfulStartPage contentfulStartPage = new ContentfulStartPageBuilder().Build();

        // Act
        StartPage result = _factory.ToModel(contentfulStartPage);

        // Assert
        Assert.NotNull(result.Alerts);
        Assert.NotNull(result.AlertsInline);
        Assert.NotNull(result.Breadcrumbs);
        _crumbFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldNotAddAlertsOrBreadcrumbs_IfTheyAreLinks()
    {
        // Arrange
        ContentfulStartPage contentfulStartPage = new ContentfulStartPageBuilder().Build();
        contentfulStartPage.Alerts.First().Sys.LinkType = "Link";
        contentfulStartPage.AlertsInline.First().Sys.LinkType = "Link";
        contentfulStartPage.Breadcrumbs.First().Sys.LinkType = "Link";

        // Act
        StartPage result = _factory.ToModel(contentfulStartPage);

        // Assert
        Assert.Empty(result.Alerts);
        Assert.Empty(result.AlertsInline);
        Assert.Empty(result.Breadcrumbs);
        _alertFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulAlert>()), Times.Never);
        _crumbFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
    }
}