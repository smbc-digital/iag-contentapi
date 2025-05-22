namespace StockportContentApiTests.Unit.ContentfulFactories;

public class HomepageContentfulFactoryTests
{
    private readonly HomepageContentfulFactory _homepageContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>> _carouselContentFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner>> _spotlightOnFactory = new();

    public HomepageContentfulFactoryTests()
    {
        _groupFactory
            .Setup(groupFactory => groupFactory.ToModel(It.IsAny<ContentfulGroup>()))
            .Returns(new GroupBuilder().Build());
        
        _subitemFactory
            .Setup(subitemFactory => subitemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItemBuilder().Build());
        
        _alertFactory
            .Setup(alertFactory => alertFactory.ToModel(It.IsAny<ContentfulAlert>()))
            .Returns(new Alert("title", string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue, string.Empty, false, string.Empty));
        
        _carouselContentFactory
            .Setup(carouselContentFactory => carouselContentFactory.ToModel(It.IsAny<ContentfulCarouselContent>()))
            .Returns(new CarouselContent(string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue, string.Empty));
        
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2017, 01, 01));
        
        _callToActionFactory
            .Setup(callToActionFactory => callToActionFactory.ToModel(It.IsAny<ContentfulCallToActionBanner>()))
            .Returns(new CallToActionBanner());
        
        _spotlightOnFactory
            .Setup(spotlightOnFactory => spotlightOnFactory.ToModel(It.IsAny<ContentfulSpotlightOnBanner>()))
            .Returns(new SpotlightOnBanner(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new DateTime()));

        _homepageContentfulFactory = new HomepageContentfulFactory(_subitemFactory.Object,
            _groupFactory.Object,
            _alertFactory.Object,
            _carouselContentFactory.Object,
            _timeProvider.Object,
            _callToActionFactory.Object,
            _spotlightOnFactory.Object);
    }

    [Fact]
    public void ShouldBuildHomepageFromContentfulHomepage()
    {
        // Arrange
        ContentfulHomepage contentfulHomepage = new ContentfulHomepageBuilder().Build();

        // Act
        Homepage homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

        // Assert
        Assert.NotNull(homepage.FeaturedGroup);
        Assert.Equal("meta description", homepage.MetaDescription);
    }

    [Fact]
    public void ShouldBuildHomepageWithNoContentfulGroup()
    {
        // Arrange
        ContentfulHomepage contentfulHomepage = new ContentfulHomepageBuilder().FeaturedGroups(new List<ContentfulGroup>()).Build();

        // Act
        Homepage homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

        // Assert
        Assert.Null(homepage.FeaturedGroup);
    }

    [Fact]
    public void ShouldPickFirstAvaliableFeaturedGroup()
    {
        // Arrange
        ContentfulHomepage contentfulHomepage = new ContentfulHomepageBuilder()
            .FeaturedGroups(new List<ContentfulGroup>()
            {
                new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(2018, 01, 01)).Build(),
                new ContentfulGroupBuilder().Slug("a-custom-slug").Build(),
                new ContentfulGroupBuilder().Build()
            }).Build();

        // Act
        Homepage homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

        // Assert
        Assert.NotNull(homepage.FeaturedGroup);
    }

    [Fact]
    public void ShouldNotFailIfNoGroupsCanBeUsed()
    {
        // Arrange
        ContentfulHomepage contentfulHomepage = new ContentfulHomepageBuilder()
            .FeaturedGroups(new List<ContentfulGroup>()
            {
                new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(3000, 01, 01)).Build(),
                new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(3000, 01, 01)).Build(),
                new ContentfulGroupBuilder().DateHiddenFrom(new DateTime(2016, 01, 01)).DateHiddenTo(new DateTime(3000, 01, 01)).Build()
            }).Build();

        // Act
        Homepage homepage = _homepageContentfulFactory.ToModel(contentfulHomepage);

        // Assert
        Assert.Null(homepage.FeaturedGroup);
    }
}