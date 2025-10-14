namespace StockportContentApiTests.Unit.Repositories;

public class LandingPageRepositoryTests
{
    private readonly Mock<ICache> _cacheWrapper = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulLandingPage, LandingPage>> _contentfulFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory = new();
    private readonly Mock<EventRepository> _eventRepository = new();
    private readonly Mock<IContentfulFactory<ContentfulNews, News>> _newsFactory = new();
    private readonly Mock<NewsRepository> _newsRepository = new();
    private readonly Mock<IContentfulFactory<ContentfulNewsRoom, Newsroom>> _newsRoomFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();
    private readonly LandingPageRepository _repository;
    private readonly Mock<ITimeProvider> _timeProvider = new();

    private readonly LandingPage _landingPage = new()
    {
        Slug = "landing-page-slug",
        Title = "landing page title",
        Subtitle = "landing page subtitle",
        Breadcrumbs = new List<Crumb>(),
        Alerts = new List<Alert>(),
        Teaser = "landing page teaser",
        MetaDescription = "landing page meta description",
        Image = new(),
        Icon = "icon",
        HeaderType = "full image",
        HeaderImage = new(),
        PageSections = new List<ContentBlock> { new() { Title = "ContentBlock 1" }, new() { Title = "ContentBlock 2" } }
    };

    public LandingPageRepositoryTests()
    {
        ContentfulConfig config = BuildContentfulConfig();

        CacheKeyConfig cacheKeyconfig = new CacheKeyConfig("test")
            .Add("TEST_EventsCacheKey", "testEventsCacheKey")
            .Add("TEST_NewsCacheKey", "testNewsCacheKey")
            .Build();

        _timeProvider
            .Setup(time => time.Now())
            .Returns(DateTime.Today.AddDays(1));
        
        Mock<IContentfulClientManager> contentfulClientManager = SetupContentfulClientManager(config);

        _configuration
            .Setup(configuration => configuration["redisExpiryTimes:Events"])
            .Returns("60");

        _eventRepository = new(config,
                            cacheKeyconfig,
                            contentfulClientManager.Object,
                            _timeProvider.Object,
                            _eventFactory.Object,
                            _eventHomepageFactory.Object,
                            _cacheWrapper.Object,
                            _configuration.Object);

        _newsRepository = new(config,
                            _timeProvider.Object,
                            contentfulClientManager.Object,
                            _newsFactory.Object,
                            _newsRoomFactory.Object,
                            _cacheWrapper.Object,
                            _configuration.Object,
                            _eventRepository.Object);
        
        _repository = new(config,
                        _contentfulFactory.Object,
                        contentfulClientManager.Object,
                        _eventRepository.Object,
                        _newsRepository.Object,
                        _profileFactory.Object);

        ContentfulLandingPage contentfulLandingPage = new ContentfulLandingPageBuilder().Build();

        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = [contentfulLandingPage] };

        _contentfulClient
            .Setup(client =>client.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        _contentfulFactory
            .Setup(factory => factory.ToModel(contentfulLandingPage))
            .Returns(_landingPage);
    }

    private static ContentfulConfig BuildContentfulConfig() =>
        new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

    private Mock<IContentfulClientManager> SetupContentfulClientManager(ContentfulConfig config)
    {
        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(client => client.GetClient(config))
            .Returns(_contentfulClient.Object);
        
        return contentfulClientManager;
    }

    [Fact]
    public async Task GetLandingPage_ReturnsSuccessResponse_WhenLandingPageIsFound()
    {
        // Act
        HttpResponse response = await _repository.GetLandingPage("landing-page-slug", "tagId");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLandingPage_ReturnsNotFoundResponse_WhenLandingPageIsNotFound()
    {
        // Arrange
        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = Enumerable.Empty<ContentfulLandingPage>() };
        
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        // Act
        HttpResponse response = await _repository.GetLandingPage("non-existent-slug", "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No landing page found", response.Error);
    }

    [Fact]
    public async Task GetLandingPage_PopulatesContentBlocks_WhenPageSectionsItemsArePresent()
    {
        // Act
        HttpResponse response = await _repository.GetLandingPage("landing-page-slug", "tagId");
        LandingPage responseLandingPage = response.Get<LandingPage>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseLandingPage.PageSections);
        Assert.Equal(2, responseLandingPage.PageSections.Count());
        Assert.Equal(_landingPage.PageSections.ToList()[0], responseLandingPage.PageSections.ToList()[0]);
        Assert.Equal(_landingPage.PageSections.ToList()[1], responseLandingPage.PageSections.ToList()[1]);
    }

    [Fact]
    public async Task GetLandingPage_GetsEventsFromCategory_WhenCategoryEventListIsNotEmpty()
    {
        // Arrange
        _landingPage.PageSections = new List<ContentBlock>
        {
            new() { ContentType = "EventCards", AssociatedTagCategory = "events" }
        };

        List<Event> events = new()
        {
            new EventBuilder().Slug("slug1").Build(),
            new EventBuilder().Slug("slug2").Build(),
            new EventBuilder().Slug("slug3").Build()
        };

        _eventRepository
            .Setup(repository => repository.GetEventsByCategory(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
            .ReturnsAsync(events);

        // Act
        HttpResponse response = await _repository.GetLandingPage("landing-page-slug", "tagId");
        List<Event> responseEvents = response.Get<LandingPage>().PageSections.First().Events;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseEvents);
        Assert.Equal(3, responseEvents.Count);
        _eventRepository.Verify(repository => repository.GetEventsByCategory(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        _eventRepository.Verify(repository => repository.GetEventsByTag(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetLandingPage_GetsEventsFromTags_WhenCategoryEventListIsEmpty()
    {
        // Arrange
        _landingPage.PageSections = new List<ContentBlock>
        {
            new() { ContentType = "EventCards", AssociatedTagCategory = "events" }
        };

        List<Event> events = new()
        {
            new EventBuilder().Slug("slug1").Build(),
            new EventBuilder().Slug("slug2").Build(),
            new EventBuilder().Slug("slug3").Build()
        };

        _eventRepository
            .Setup(repository => repository.GetEventsByCategory(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Event>());

        _eventRepository
            .Setup(repository => repository.GetEventsByTag(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
            .ReturnsAsync(events);

        // Act
        HttpResponse response = await _repository.GetLandingPage("landing-page-slug", "tagId");
        List<Event> responseEvents = response.Get<LandingPage>().PageSections.First().Events;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseEvents);
        Assert.Equal(3, responseEvents.Count);
        _eventRepository.Verify(repository => repository.GetEventsByCategory(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        _eventRepository.Verify(repository => repository.GetEventsByTag(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetLandingPage_ShouldCallGetLatestNewsByCategory_WhenNewsBannerExists()
    {
        // Arrange
        _landingPage.PageSections = new List<ContentBlock>
        {
            new() { ContentType = "NewsBanner", AssociatedTagCategory = "some-category" }
        };

        List<News> news = new()
        {
            new(It.IsAny<string>(),
                "slug",
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<List<Alert>>(),
                It.IsAny<List<string>>(),
                new List<string>() { "some-category" },
                It.IsAny<List<InlineQuote>>(),
                It.IsAny<CallToActionBanner>(),
                It.IsAny<string>(),
                It.IsAny<List<TrustedLogo>>(),
                It.IsAny<TrustedLogo>(),
                It.IsAny<string>())
        };

        _newsRepository
            .Setup(repository => repository.GetLatestNewsByCategory(It.IsAny<string>(), "tagId", 1))
            .ReturnsAsync(news);

        // Act
        HttpResponse result = await _repository.GetLandingPage("landing-page-slug", "tagId");
        News responseNews = result.Get<LandingPage>().PageSections.FirstOrDefault().NewsArticle;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(news.FirstOrDefault(), responseNews);
        _newsRepository.Verify(repository => repository.GetLatestNewsByCategory(It.IsAny<string>(), "tagId", 1), Times.Once);
        _newsRepository.Verify(repository => repository.GetLatestNewsByTag(It.IsAny<string>(), "tagId", 1), Times.Once);
    }

    [Fact]
    public async Task GetLandingPage_ShouldReturnNews_WhenTagMatches()
    {
        // Arrange
        _landingPage.PageSections = new List<ContentBlock>
        {
            new() { ContentType = "NewsBanner", AssociatedTagCategory = "some-tag" }
        };

        List<News> news = new()
        {
            new News(It.IsAny<string>(),
                    "slug",
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<List<Alert>>(),
                    new List<string>() { "some-tag" },
                    It.IsAny<List<string>>(),
                    It.IsAny<List<InlineQuote>>(),
                    It.IsAny<CallToActionBanner>(),
                    It.IsAny<string>(),
                    It.IsAny<List<TrustedLogo>>(),
                    It.IsAny<TrustedLogo>(),
                    It.IsAny<string>())
        };

        _newsRepository
            .Setup(repository => repository.GetLatestNewsByCategory(It.IsAny<string>(), "tagId", 1))
            .ReturnsAsync((List<News>)null);

        _newsRepository
            .Setup(repository => repository.GetLatestNewsByTag(It.IsAny<string>(), "tagId", 1))
            .ReturnsAsync(news);
    
        // Act
        HttpResponse result = await _repository.GetLandingPage("landing-page-slug", "tagId");
        News responseNews = result.Get<LandingPage>().PageSections.FirstOrDefault().NewsArticle;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(news.FirstOrDefault(), responseNews);
        _newsRepository.Verify(repository => repository.GetLatestNewsByTag(It.IsAny<string>(), "tagId", 1), Times.Exactly(2));
        _newsRepository.Verify(repository => repository.GetLatestNewsByCategory(It.IsAny<string>(), "tagId", 1), Times.Once);
    }

    [Fact]
    public async Task PopulateNewsContent_ShouldUseNewsSubItem_WhenValidSubItemExists()
    {
        // Arrange
        var subItemSlug = "news-subitem-slug";
        List<News> news = new()
        {
            new News("this is the title",
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<List<Alert>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<InlineQuote>>(),
                    It.IsAny<CallToActionBanner>(),
                    It.IsAny<string>(),
                    It.IsAny<List<TrustedLogo>>(),
                    It.IsAny<TrustedLogo>(),
                    It.IsAny<string>())
        };

        _landingPage.PageSections = new List<ContentBlock>
        {
            new()
            {
                ContentType = "NewsBanner",
                AssociatedTagCategory = "some-tag",
                SubItems = new List<ContentBlock>
                {
                    new() { Slug = subItemSlug }
                }
            }
        };

        _newsRepository
            .Setup(repo => repo.GetNews(subItemSlug, "tagId"))
            .ReturnsAsync(HttpResponse.Successful(news.FirstOrDefault()));

        _newsRepository
            .Setup(repo => repo.GetNews("news-subitem-slug", "tagId"))
            .ReturnsAsync(HttpResponse.Successful(news.First()));

        // Act
        HttpResponse result = await _repository.GetLandingPage("landing-page-slug", "tagId");
        ContentBlock section = result.Get<LandingPage>().PageSections.FirstOrDefault();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(section.NewsArticle);
        Assert.Equal(news.FirstOrDefault(), section.NewsArticle);
        Assert.False(section.UseTag);
        _newsRepository.Verify(repo => repo.GetNews(subItemSlug, "tagId"), Times.Once);
        _newsRepository.Verify(repo => repo.GetLatestNewsByCategory(It.IsAny<string>(), "tagId", 1), Times.Never);
        _newsRepository.Verify(repo => repo.GetLatestNewsByTag(It.IsAny<string>(), "tagId", 1), Times.Never);
    }

    [Fact]
    public async Task GetProfile_ReturnsProfile_WhenProfileExists()
    {
        // Arrange
        ContentfulCollection<ContentfulProfile> profiles = new()
        {
            Items = new List<ContentfulProfile> { new ContentfulProfileBuilder().Slug("test-slug").Build() }
        };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profiles);

        // Act
        ContentfulProfile result = await _repository.GetProfile("test-slug", "tagId");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-slug", result.Slug);
    }

    [Fact]
    public async Task GetProfile_ReturnsNull_WhenNoProfileExists()
    {
        // Arrange
        ContentfulCollection<ContentfulProfile> profiles = new() { Items = new List<ContentfulProfile>() };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profiles);

        // Act
        ContentfulProfile result = await _repository.GetProfile("non-existent-slug", "tagId");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProfile_ThrowsException_WhenContentfulClientFails()
    {
        // Arrange
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new("Contentful service error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.GetProfile("test-slug", "tagId"));
    }

    [Fact]
    public async Task GetLandingPage_ShouldHandleProfileBannerContentBlock()
    {
        // Arrange
        _landingPage.PageSections = new List<ContentBlock>
        {
            new()
            {
                ContentType = "ProfileBanner",
                SubItems = new List<ContentBlock>
                {
                    new() { Slug = "profile-slug"}
                }
            }
        };

        ContentfulProfile mockContentfulProfile = new() { Slug = "profile-slug" };
        Profile mockProfile = new() { Slug = "profile-slug" };
        
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulProfile>
            {
                Items = new List<ContentfulProfile> { mockContentfulProfile }
            });
        
        _profileFactory
            .Setup(factory => factory.ToModel(mockContentfulProfile))
            .Returns(mockProfile);

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug", "tagId");
        ContentBlock contentBlock = response.Get<LandingPage>().PageSections.FirstOrDefault();

        // Assert
        Assert.NotNull(contentBlock);
        Assert.Equal("ProfileBanner", contentBlock.ContentType);
        Assert.NotNull(contentBlock.Profile);
        Assert.Equal(mockProfile, contentBlock.Profile);
        _contentfulClient.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()), Times.Once);
        _profileFactory.Verify(factory => factory.ToModel(mockContentfulProfile), Times.Once);
    }

    [Fact]
    public async Task GetLandingPage_ShouldHandleOtherContentTypeWithDefaultCase()
    {
        // Arrange
        _landingPage.PageSections = new List<ContentBlock>
        {
            new()
            {
                ContentType = "TriviaList",
                SubItems = new List<ContentBlock>
                {
                    new() { Title = "trivia title"}
                }
            }
        };

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug", "tagId");
        ContentBlock contentBlock = response.Get<LandingPage>().PageSections.FirstOrDefault();

        // Assert
        Assert.NotNull(contentBlock);
        Assert.Equal("TriviaList", contentBlock.ContentType);
        Assert.Null(contentBlock.NewsArticle);
        Assert.Null(contentBlock.Events);
        Assert.Null(contentBlock.Profile);
    }
}