using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class LandingPageContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly ITimeProvider _timeProvider = new TimeProvider();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, ContentBlock>> _contentBlockFactory = new();
    private readonly IContentfulFactory<ContentfulLandingPage, LandingPage> _landingPageFactory;
    private readonly ContentfulLandingPage _contentfulLandingPage = new()
    {
        Slug = "landing-page-slug",
        Title = "landing page title",
        Subtitle = "landing page subtitle",
        Breadcrumbs = new List<ContentfulReference>() { new ContentfulReferenceBuilder().Build() },
        Alerts = new List<ContentfulAlert>(),
        Teaser = "landing page teaser",
        MetaDescription = "landing page metadescription",
        Icon = "icon",
        Image = new Asset(),
        HeaderType = "full image",
        HeaderImage = new Asset(),
    };

    public LandingPageContentfulFactoryTests() => _landingPageFactory = new LandingPageContentfulFactory(_crumbFactory.Object, _timeProvider, _alertFactory.Object, _contentBlockFactory.Object);

    [Fact]
    public void ToModel_ShouldCreateALandingPageFromAContentfulLandingPage()
    {
        // Arrange
        Crumb crumb = new("title", "slug", "type");
        Alert alert = new("title", "subheading", "body", "test", new DateTime(2017, 01, 01), new DateTime(2017, 04, 10), string.Empty, false, string.Empty);
        _contentfulLandingPage.Breadcrumbs = new List<ContentfulReference>() { new ContentfulReferenceBuilder().Build() };
        _contentfulLandingPage.Alerts = new List<ContentfulAlert>() { new ContentfulAlertBuilder().Build() };
        _contentfulLandingPage.PageSections = new List<ContentfulReference>() { new ContentfulReferenceBuilder().Build() };

        _crumbFactory.Setup(_ => _.ToModel(_contentfulLandingPage.Breadcrumbs.First())).Returns(crumb);
        _alertFactory.Setup(_ => _.ToModel(_contentfulLandingPage.Alerts.First())).Returns(alert);

        // Act
        LandingPage result = _landingPageFactory.ToModel(_contentfulLandingPage);

        // Assert
        Assert.Equal("landing-page-slug", result.Slug);
        Assert.Equal("landing page title", result.Title);
        Assert.Equal("landing page subtitle", result.Subtitle);
        Assert.Equal("landing page teaser", result.Teaser);
        Assert.Equal("landing page metadescription", result.MetaDescription);
        Assert.Equal("icon", result.Icon);
        Assert.Equal("full image", result.HeaderType);
        Assert.Equal(crumb, result.Breadcrumbs.First());
        _crumbFactory.Verify(_ => _.ToModel(_contentfulLandingPage.Breadcrumbs.First()), Times.Once);
        Assert.Equal(alert, result.Alerts.First());
        _alertFactory.Verify(_ => _.ToModel(_contentfulLandingPage.Alerts.First()), Times.Once);
        _contentBlockFactory.Verify(_ => _.ToModel(_contentfulLandingPage.PageSections.First()), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldNotAddBreadcrumbsOrAlertsOrPageSections_If_TheyAreLinks()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new()
        {
            Slug = "landing-page-slug",
            Title = "landing page title",
            Subtitle = "landing page subtitle",
            Breadcrumbs = new List<ContentfulReference>() { new ContentfulReferenceBuilder().Build() },
            Alerts = new List<ContentfulAlert>() { new ContentfulAlertBuilder().Build() },
            Teaser = "landing page teaser",
            MetaDescription = "landing page metaDescription",
            Icon = "icon",
            Image = new ContentfulAssetBuilder().Url("image-url.jpg").Build(),
            HeaderType = "full image",
            HeaderImage = new ContentfulAssetBuilder().Url("header-image-url.jpg").Build(),
            PageSections = new() { new ContentfulReferenceBuilder().Build() }
        };

        contentfulLandingPage.Breadcrumbs.First().Sys.LinkType = "Link";
        contentfulLandingPage.Alerts.First().Sys.LinkType = "Link";
        contentfulLandingPage.PageSections.First().Sys.LinkType = "Link";

        // Act
        LandingPage landingPage = _landingPageFactory.ToModel(contentfulLandingPage);

        // Assert
        Assert.Empty(landingPage.Breadcrumbs);
        Assert.Empty(landingPage.Alerts);
        _crumbFactory.Verify(_ => _.ToModel(contentfulLandingPage.Breadcrumbs.First()), Times.Never);
        _alertFactory.Verify(_ => _.ToModel(contentfulLandingPage.Alerts.First()), Times.Never);
        _contentBlockFactory.Verify(_ => _.ToModel(contentfulLandingPage.PageSections.First()), Times.Never);
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