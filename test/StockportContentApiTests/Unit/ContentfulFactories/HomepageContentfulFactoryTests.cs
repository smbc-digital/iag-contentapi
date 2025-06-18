namespace StockportContentApiTests.Unit.ContentfulFactories;

public class HomepageContentfulFactoryTests
{
    private readonly HomepageContentfulFactory _homepageContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>> _carouselContentFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner>> _spotlightOnFactory = new();

    public HomepageContentfulFactoryTests()
    {
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
            .Returns(new SpotlightOnBanner(string.Empty, null, string.Empty, string.Empty, new DateTime()));

        _homepageContentfulFactory = new HomepageContentfulFactory(_subitemFactory.Object,
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
        Assert.Equal("meta description", homepage.MetaDescription);
    }
}