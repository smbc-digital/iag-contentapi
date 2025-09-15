using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class LandingPageContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly ITimeProvider _timeProvider = new TimeProvider();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, ContentBlock>> _contentBlockFactory = new();
    readonly ContentfulLandingPage _contentfulLandingPage = new ContentfulLandingPageBuilder().Build();
    private readonly LandingPageContentfulFactory _landingPageFactory;

    public LandingPageContentfulFactoryTests() =>
            _landingPageFactory = new LandingPageContentfulFactory(_crumbFactory.Object,
                                                                _timeProvider,
                                                                _alertFactory.Object,
                                                                _contentBlockFactory.Object);

    [Fact]
    public void ToModel_ShouldCreateALandingPageFromAContentfulLandingPage()
    {
        // Arrange
        Crumb crumb = new("title", "slug", "type");
        Alert alert = new("title", "body", "test", new DateTime(2017, 01, 01), new DateTime(2017, 04, 10), string.Empty, false, string.Empty);
        _contentfulLandingPage.Breadcrumbs = new List<ContentfulReference>() { new ContentfulReferenceBuilder().Build() };
        _contentfulLandingPage.Alerts = new List<ContentfulAlert>() { new ContentfulAlertBuilder().Build() };
        _contentfulLandingPage.PageSections = new List<ContentfulReference>() { new ContentfulReferenceBuilder().Build() };

        _crumbFactory
            .Setup(crumbFactory => crumbFactory.ToModel(_contentfulLandingPage.Breadcrumbs.First()))
            .Returns(crumb);

        _alertFactory
            .Setup(alertFactory => alertFactory.ToModel(_contentfulLandingPage.Alerts.First()))
            .Returns(alert);

        // Act
        LandingPage result = _landingPageFactory.ToModel(_contentfulLandingPage);

        // Assert
        Assert.Equal("slug", result.Slug);
        Assert.Equal("title", result.Title);
        Assert.Equal("subtitle", result.Subtitle);
        Assert.Equal("teaser", result.Teaser);
        Assert.Equal("meta description", result.MetaDescription);
        Assert.Equal("icon", result.Icon);
        Assert.Equal("header type", result.HeaderType);
        Assert.Equal(crumb, result.Breadcrumbs.First());
        Assert.Equal(alert, result.Alerts.First());
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(_contentfulLandingPage.Breadcrumbs.First()), Times.Once);
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(_contentfulLandingPage.Alerts.First()), Times.Once);
        _contentBlockFactory.Verify(contentBlockFactory => contentBlockFactory.ToModel(_contentfulLandingPage.PageSections.First()), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldNotAddBreadcrumbsOrAlertsOrPageSections_If_TheyAreLinks()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new ContentfulLandingPageBuilder().Build();

        contentfulLandingPage.Breadcrumbs.First().Sys.LinkType = "Link";
        contentfulLandingPage.Alerts.First().Sys.LinkType = "Link";
        contentfulLandingPage.PageSections.First().Sys.LinkType = "Link";

        // Act
        LandingPage landingPage = _landingPageFactory.ToModel(contentfulLandingPage);

        // Assert
        Assert.Empty(landingPage.Breadcrumbs);
        Assert.Empty(landingPage.Alerts);
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(contentfulLandingPage.Breadcrumbs.First()), Times.Never);
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(contentfulLandingPage.Alerts.First()), Times.Never);
        _contentBlockFactory.Verify(contentBlockFactory => contentBlockFactory.ToModel(contentfulLandingPage.PageSections.First()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldCreateALandingPageFromAContentfulLandingPage_And_FilterAlertsWithOneOutsideOfDates()
    {
        // Arrange
        List<ContentfulAlert> alerts = new() {
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2017, 04, 10)).WithSunriseDate(new DateTime(2017, 01, 01)).Build(),
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2117, 01, 04)).WithSunriseDate(new DateTime(2017, 01, 01)).Build()
        };

        _contentfulLandingPage.Alerts = alerts;

        // Act
        LandingPage landingPage = _landingPageFactory.ToModel(_contentfulLandingPage);

        // Arrange
        Assert.Single(landingPage.Alerts);
    }

    [Fact]
    public void ToModel_ShouldCreateALandingPageFromAContentfulLandingPage_And_FilterAlertsWithSeverityOfCondolence()
    {
        // Arrange
        List<ContentfulAlert> alerts = new() {
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2117, 04, 10)).WithSunriseDate(new DateTime(2017, 01, 01)).WithSeverity("Information").Build(),
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2117, 04, 10)).WithSunriseDate(new DateTime(2017, 01, 01)).WithSeverity("Condolence").Build()
        };

        _contentfulLandingPage.Alerts = alerts;

        // Act
        LandingPage landingPage = _landingPageFactory.ToModel(_contentfulLandingPage);

        // Assert
        Assert.Single(landingPage.Alerts);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_IfNullEntry()
    {
        // Act & Assert
        Assert.Null(_landingPageFactory.ToModel(null));
    }

    [Fact]
    public void ToModel_MapsImagesCorrectly()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new()
        {
            Image = new Asset
            {
                File = new File { Url = "image-url" },
                Description = "Image description"
            },
            HeaderImage = new Asset
            {
                File = new File { Url = "header-url" },
                Description = "Header description"
            }
        };

        // Act
        LandingPage result = _landingPageFactory.ToModel(contentfulLandingPage);

        // Assert
        Assert.Equal("image-url", result.Image.Url);
        Assert.Equal("Image description", result.Image.Description);
        Assert.Equal("header-url", result.HeaderImage.Url);
        Assert.Equal("Header description", result.HeaderImage.Description);
    }
}