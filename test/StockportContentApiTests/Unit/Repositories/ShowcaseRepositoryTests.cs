namespace StockportContentApiTests.Unit.Repositories;

public class ShowcaseRepositoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private readonly Mock<ICache> _cacheWrapper;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory;
    private readonly Mock<ILogger<ShowcaseRepository>> _mockLogger;
    private readonly ShowcaseRepository _repository;
    private readonly Mock<ITimeProvider> _timeprovider;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _topicFactory;

    public ShowcaseRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        CacheKeyConfig cacheKeyconfig = new CacheKeyConfig("test")
            .Add("TEST_EventsCacheKey", "testEventsCacheKey")
            .Add("TEST_NewsCacheKey", "testNewsCacheKey")
            .Build();

        _topicFactory = new();
        _crumbFactory = new();
        _timeprovider = new();
        _eventHomepageFactory = new();
        _alertFactory = new();
        _mockLogger = new();
        _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

        Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>> socialMediaFactory = new();
        socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(
            new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url",
                "sm-link-accountName", "sm-link-screenReader"));

        _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title",
            string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue, string.Empty, false, string.Empty));

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

        Mock<IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner>> spotlightBannerFactory = new();

        ShowcaseContentfulFactory contentfulFactory = new(_topicFactory.Object, _crumbFactory.Object,
            _timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object,
            _triviaFactory.Object, callToActionBanner.Object, _videoFactory.Object, spotlightBannerFactory.Object);

        Mock<IContentfulFactory<ContentfulNews, News>> newsListFactory = new();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        _eventFactory = new();
        _cacheWrapper = new();

        _configuration = new();
        _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");

        EventRepository eventRepository = new(config, cacheKeyconfig, contentfulClientManager.Object, _timeprovider.Object,
            _eventFactory.Object, _eventHomepageFactory.Object, _cacheWrapper.Object,
            _configuration.Object);

        _repository = new(config, contentfulFactory, contentfulClientManager.Object, newsListFactory.Object,
            eventRepository, _mockLogger.Object);
    }

    [Fact]
    public void ItGetsShowcase()
    {
        // Arrange
        const string slug = "unit-test-showcase";

        ContentfulEvent rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new(2017, 4, 1)).Build();
        List<ContentfulEvent> events = new() { rawEvent };

        Event modelledEvent = new("title",
                                "event-slug",
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                DateTime.MaxValue,
                                string.Empty,
                                string.Empty,
                                1,
                                EventFrequency.None,
                                null,
                                string.Empty,
                                null,
                                new(),
                                false,
                                string.Empty,
                                DateTime.MinValue,
                                new(),
                                null,
                                new() { new("event", "slug", "icon", string.Empty) },
                                null,
                                null,
                                string.Empty,
                                null,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                null,
                                string.Empty,
                                string.Empty,
                                new List<CallToActionBanner>());

        _eventFactory.Setup(e => e.ToModel(It.IsAny<ContentfulEvent>())).Returns(modelledEvent);

        ContentfulCollection<ContentfulShowcase> collection = new();
        ContentfulShowcase rawShowcase = new ContentfulShowcaseBuilder().Slug(slug).Build();
        collection.Items = new List<ContentfulShowcase> { rawShowcase };

        QueryBuilder<ContentfulShowcase> builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase")
            .FieldEquals("fields.slug", slug).Include(3);

        _contentfulClient.Setup(o =>
                o.GetEntries(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build().Equals(builder.Build())),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
            It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(events);

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
            .Breadcrumbs(new()
            {
                new() { Title = crumb.Title, Slug = crumb.Title, Sys = new() { Type = "Entry" } }
            })
            .Build();
        collection.Items = new List<ContentfulShowcase> { rawShowcase };

        QueryBuilder<ContentfulShowcase> builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase")
            .FieldEquals("fields.slug", slug).Include(3);
        _contentfulClient.Setup(o =>
                o.GetEntries(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build().Equals(builder.Build())),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(crumb);

        ContentfulEvent rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new(2017, 4, 1)).Build();
        List<ContentfulEvent> events = new() { rawEvent };
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
            It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(events);
        Event modelledEvent = new("title",
                                "event-slug",
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                DateTime.MaxValue,
                                string.Empty,
                                string.Empty,
                                1,
                                EventFrequency.None,
                                null,
                                string.Empty,
                                new(),
                                null,
                                false,
                                string.Empty,
                                DateTime.MinValue,
                                new(),
                                null,
                                new() { new("event", "slug", "icon", string.Empty) },
                                null,
                                null,
                                string.Empty,
                                null,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                null,
                                string.Empty,
                                string.Empty,
                                new List<CallToActionBanner>());
        _eventFactory.Setup(e => e.ToModel(It.IsAny<ContentfulEvent>())).Returns(modelledEvent);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetShowcases(slug));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Showcase showcase = response.Get<Showcase>();

        showcase.Breadcrumbs.First().Should().Be(crumb);
    }
}