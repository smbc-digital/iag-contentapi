namespace StockportContentApiTests.Unit.Repositories;

public class ShowcaseRepositoryTests
{
    private readonly ShowcaseRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _topicFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;

    private readonly Mock<ITimeProvider> _timeprovider;
    private readonly Mock<ICache> _cacheWrapper;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<ILogger<ShowcaseRepository>> _mockLogger;

    public ShowcaseRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _topicFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
        _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
        _timeprovider = new Mock<ITimeProvider>();
        _eventHomepageFactory = new Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>();
        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _mockLogger = new Mock<ILogger<ShowcaseRepository>>();
        _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

        Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>> socialMediaFactory = new();
        socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

        _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue, string.Empty, false, string.Empty));

        Mock<IContentfulFactory<ContentfulVideo, Video>> _videoFactory = new();

        Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();

        Mock<IContentfulFactory<ContentfulTrivia, Trivia>> _triviaFactory = new();

        Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> callToActionBanner = new();
        callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
            new CallToActionBanner
            {
                Title = "title",
                AltText = "altText",
                ButtonText = "button text",
                Image = "url",
                Link = "url"
            });

        Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>> spotlightBannerFactory = new();

        ShowcaseContentfulFactory contentfulFactory = new(_topicFactory.Object, _crumbFactory.Object, _timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, _videoFactory.Object, spotlightBannerFactory.Object);

        Mock<IContentfulFactory<ContentfulNews, News>> newsListFactory = new();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
        _cacheWrapper = new Mock<ICache>();

        Mock<ILogger<EventRepository>> _logger = new();
        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");

        EventRepository eventRepository = new(config, contentfulClientManager.Object, _timeprovider.Object, _eventFactory.Object, _eventHomepageFactory.Object, _cacheWrapper.Object, _logger.Object, _configuration.Object);

        _repository = new ShowcaseRepository(config, contentfulFactory, contentfulClientManager.Object, newsListFactory.Object, eventRepository, _mockLogger.Object);
    }

    [Fact]
    public void ItGetsShowcase()
    {
        // Arrange
        const string slug = "unit-test-showcase";

        ContentfulEvent rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();
        List<ContentfulEvent> events = new() { rawEvent };

        Event modelledEvent = new("title", "event-slug", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MaxValue, string.Empty, string.Empty, 1, EventFrequency.None, null, string.Empty, null, new List<string>(), null, false, string.Empty, DateTime.MinValue, new List<string>(), null, null, new List<EventCategory> { new("event", "slug", "icon") }, null, null, null, null);
        _eventFactory.Setup(e => e.ToModel(It.IsAny<ContentfulEvent>())).Returns(modelledEvent);

        ContentfulCollection<ContentfulShowcase> collection = new();
        ContentfulShowcase rawShowcase = new ContentfulShowcaseBuilder().Slug(slug).Build();
        collection.Items = new List<ContentfulShowcase> { rawShowcase };

        QueryBuilder<ContentfulShowcase> builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);

        _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("event-all")), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(events);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetShowcases(slug));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void ItReturnsBreadcrumbs()
    {
        // Arrange
        const string slug = "unit-test-showcase-crumbs";
        Crumb crumb = new("title", "slug", "type");
        ContentfulCollection<ContentfulShowcase> collection = new();
        ContentfulShowcase rawShowcase = new ContentfulShowcaseBuilder().Slug(slug)
            .Breadcrumbs(new List<ContentfulReference>()
                        { new() {Title = crumb.Title, Slug = crumb.Title, Sys = new SystemProperties() {Type = "Entry" }},
                        })
            .Build();
        collection.Items = new List<ContentfulShowcase> { rawShowcase };

        QueryBuilder<ContentfulShowcase> builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);
        _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(crumb);

        ContentfulEvent rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();
        List<ContentfulEvent> events = new() { rawEvent };
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("event-all")), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(events);
        Event modelledEvent = new("title", "event-slug", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MaxValue, string.Empty, string.Empty, 1, EventFrequency.None, null, string.Empty, null, new List<string>(), null, false, string.Empty, DateTime.MinValue, new List<string>(), null, null, new List<EventCategory> { new("event", "slug", "icon") }, null, null, null, null);
        _eventFactory.Setup(e => e.ToModel(It.IsAny<ContentfulEvent>())).Returns(modelledEvent);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetShowcases(slug));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Showcase showcase = response.Get<Showcase>();

        showcase.Breadcrumbs.First().Should().Be(crumb);
    }
}
