namespace StockportContentApiTests.Unit.ContentfulFactories;

public class HomepageContentfulFactoryTests
{
    private readonly HomepageContentfulFactory _homepageContentfulFactory;
    private readonly Mock<ISubItemContentfulFactory> _subitemFactory = new();
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

        _subitemFactory
            .Setup(subitemFactory => subitemFactory.ToCacheModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItemBuilder().Build());

        _subitemFactory
            .Setup(subitemFactory => subitemFactory.FilterForCurrentDate(It.IsAny<SubItem>()))
            .Returns<SubItem>(subItem => subItem);
        
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

    [Fact]
    public void FilterForCurrentDate_ShouldReevaluateCachedHomepageContent()
    {
        DateTime now = new(2017, 01, 01);
        DateTime sunriseDate = now.AddDays(1);
        _timeProvider.Setup(timeProvider => timeProvider.Now()).Returns(() => now);

        ContentfulReference featuredTask = new ContentfulReferenceBuilder().Build();
        ContentfulReference featuredTopic = new ContentfulReferenceBuilder().Build();
        featuredTask.SunriseDate = sunriseDate;
        featuredTopic.SunriseDate = sunriseDate;
        SubItem cachedSubItem = new SubItemBuilder().Build();
        cachedSubItem.SunriseDate = sunriseDate;

        ContentfulAlert contentfulAlert = new ContentfulAlertBuilder().WithSunriseDate(sunriseDate).Build();
        Alert cachedAlert = new("title",
            string.Empty,
            string.Empty,
            sunriseDate,
            DateTime.MaxValue,
            string.Empty,
            false,
            string.Empty);
        ContentfulCarouselContent contentfulCarousel = new ContentfulCarouselContentBuilder().Build();
        contentfulCarousel.SunriseDate = sunriseDate;
        CarouselContent cachedCarousel = new(string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            sunriseDate,
            DateTime.MaxValue,
            string.Empty);

        _subitemFactory.Setup(factory => factory.ToCacheModel(featuredTask)).Returns(cachedSubItem);
        _subitemFactory.Setup(factory => factory.ToCacheModel(featuredTopic)).Returns(cachedSubItem);
        _alertFactory.Setup(factory => factory.ToModel(contentfulAlert)).Returns(cachedAlert);
        _carouselContentFactory.Setup(factory => factory.ToModel(contentfulCarousel)).Returns(cachedCarousel);

        ContentfulHomepage entry = new ContentfulHomepageBuilder().Build();
        entry.FeaturedTasks = new List<ContentfulReference> { featuredTask };
        entry.FeaturedTopics = new List<ContentfulReference> { featuredTopic };
        entry.Alerts = new List<ContentfulAlert> { contentfulAlert };
        entry.CarouselContents = new List<ContentfulCarouselContent> { contentfulCarousel };

        Homepage cached = _homepageContentfulFactory.ToCacheModel(entry);

        Assert.Single(cached.FeaturedTasks);
        Assert.Single(cached.FeaturedTopics);
        Assert.Single(cached.Alerts);
        Assert.Single(cached.CarouselContents);

        Homepage hidden = _homepageContentfulFactory.FilterForCurrentDate(cached);
        Assert.Empty(hidden.FeaturedTasks);
        Assert.Empty(hidden.FeaturedTopics);
        Assert.Empty(hidden.Alerts);
        Assert.Empty(hidden.CarouselContents);

        now = sunriseDate;
        Homepage visible = _homepageContentfulFactory.FilterForCurrentDate(cached);

        Assert.Single(visible.FeaturedTasks);
        Assert.Single(visible.FeaturedTopics);
        Assert.Single(visible.Alerts);
        Assert.Single(visible.CarouselContents);
        Assert.Single(cached.FeaturedTasks);
        Assert.Single(cached.Alerts);
    }
}