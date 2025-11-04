namespace StockportContentApiTests.Unit.Repositories;

public class NewsRepositoryTests
{
    private readonly List<Alert> _alerts = new()
    {
        new("title",
            "body",
            "severity",
            new(2016, 08, 5),
            new(2016, 08, 11),
            string.Empty,
            false,
            string.Empty)
    };

    private readonly Mock<ICache> _cacheWrapper = new();
    private readonly Mock<IContentfulClient> _client = new();

    private readonly ContentfulConfig _config = new ContentfulConfig("test")
        .Add("DELIVERY_URL", "https://fake.url")
        .Add("TEST_SPACE", "SPACE")
        .Add("TEST_ACCESS_KEY", "KEY")
        .Add("TEST_MANAGEMENT_KEY", "KEY")
        .Add("TEST_ENVIRONMENT", "master")
        .Build();

    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IContentfulClientManager> _contentfulClientManager = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly List<string> _newsCategories = new() { "news-category-1", "news-category-2" };
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulNews, News>> _newsFactory = new();
    private readonly ContentType _newsContentType;
    private readonly ContentfulCollection<ContentfulNewsRoom> _newsroomContentfulCollection;
    private readonly Mock<IContentfulFactory<ContentfulNewsRoom, Newsroom>> _newsRoomContentfulFactory = new();
    private readonly NewsRepository _repository;
    private readonly DateTime _sunriseDate = new(2016, 08, 01);
    private readonly DateTime _sunsetDate = new(2016, 08, 10);
    private readonly DateTime _updatedAt = new(2016, 08, 05);
    private readonly Mock<IVideoRepository> _videoRepository = new();
    private readonly Mock<EventRepository> _eventRepository = new();
    private readonly ContentfulNews contentfulNews = new ContentfulNewsBuilder()
        .Title("This is the news")
        .Body("The news")
        .Teaser("Read more")
        .Slug("news-of-the-century")
        .SunriseDate(new(2016, 08, 01))
        .SunsetDate(new(2016, 08, 10))
        .Build();

    private readonly ContentfulNews contentfulNews2 = new ContentfulNewsBuilder()
        .Title("This is a news with no sunrise/sunset")
        .Slug("news-with-no-sunrise-sunset")
        .Teaser("Read more")
        .SunriseDate(DateTime.MinValue.ToUniversalTime())
        .SunsetDate(DateTime.MinValue.ToUniversalTime())
        .Categories(new List<string>() { "Benefits" })
        .Tags(new List<string>() { "sports" })
        .Build();

    private readonly ContentfulNews contentfulNews3 = new ContentfulNewsBuilder()
        .Title("Another news article")
        .Slug("another-news-article")
        .Teaser("This is another news article")
        .SunriseDate(new DateTime(2016, 06, 30, 23, 0, 0, DateTimeKind.Utc))
        .SunsetDate(new DateTime(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc))
        .Categories(new List<string>() { "Benefits" })
        .Build();

    private readonly News expectedNews = new(It.IsAny<string>(),
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
                                It.IsAny<string>());

    private readonly List<ContentfulNews> contentfulNewsList = new()
    {
        new()
        {
            Categories = new() { "sports" },
            SunriseDate = DateTime.Now.AddDays(-1),
            SunsetDate = DateTime.Now.AddDays(1)
        }
    };

    public NewsRepositoryTests()
    {
        _newsContentType = new()
        {
            Fields = new()
                {
                    new()
                    {
                        Name = "Categories",
                        Items = new()
                        {
                            Validations = new()
                            {
                                new InValuesValidator
                                    { RequiredValues = new() { "Benefits", "Business", "Council leader" } }
                            }
                        }
                    }
                }
        };

        _newsroomContentfulCollection = new()
        {
            Items = new List<ContentfulNewsRoom>
                {
                    new ContentfulNewsRoomBuilder().Build()
                }
        };

        _contentfulClientManager
            .Setup(contentfulClient => contentfulClient.GetClient(_config))
            .Returns(_client.Object);

        _client.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNewsRoom>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_newsroomContentfulCollection);

        _client
            .Setup(client => client.GetContentType("news", It.IsAny<CancellationToken>()))
            .ReturnsAsync(_newsContentType);

        _configuration
            .Setup(conf => conf["redisExpiryTimes:News"])
            .Returns("60");

        CacheKeyConfig cacheKeyconfig = new CacheKeyConfig("test")
            .Add("TEST_EventsCacheKey", "testEventsCacheKey")
            .Add("TEST_NewsCacheKey", "testNewsCacheKey")
            .Build();

        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(DateTime.Now);

        _eventRepository = new(_config,
                            cacheKeyconfig,
                            _contentfulClientManager.Object,
                            _timeProvider.Object,
                            _eventFactory.Object,
                            _eventHomepageFactory.Object,
                            _cacheWrapper.Object,
                            _configuration.Object);

        _repository = new(_config,
                        _timeProvider.Object,
                        _contentfulClientManager.Object,
                        _newsFactory.Object,
                        _newsRoomContentfulFactory.Object,
                        _cacheWrapper.Object,
                        _configuration.Object,
                        _eventRepository.Object);
    }

    [Fact]
    public async Task GetsNews_ShouldReturnItemFromASlug()
    {
        // Arrange
        List<Alert> alerts = new() { new AlertBuilder().Build() };

        ContentfulCollection<ContentfulNews> collection = new()
        {
            Items = new List<ContentfulNews> { contentfulNews }
        };

        List<ContentfulNews> newsCollection = new() { contentfulNews };

        string simpleNewsQuery = new QueryBuilder<ContentfulNews>()
            .ContentTypeIs("news")
            .FieldEquals("fields.slug", "news-of-the-century")
            .Include(1)
            .Build();

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _videoRepository
            .Setup(videoRepo => videoRepo.Process(It.IsAny<string>()))
            .Returns(contentfulNews.Body);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsCollection);
            
        News news = CreateTestNews("This is the news", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        // Act
        HttpResponse response = await _repository.GetNews("news-of-the-century", "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(news, response.Get<News>());
    }

    [Fact]
    public async Task GetNews_ShouldReturnNotFound_If_NewsWithSlugNotFound()
    {
        // Arrange
        ContentfulCollection<ContentfulNews> collection = new()
        {
            Items = new List<ContentfulNews>()
        };

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(collection.Items.ToList());

        // Act
        HttpResponse response = await _repository.GetNews("news-of-the-century", "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No news found for 'news-of-the-century'", response.Error);
    }

    [Fact]
    public async Task GetNews_ShouldCallGetAllNews_If_NewsNotCached()
    {
        // Arrange
        List<ContentfulNews> expectedNewsList = new()
        {
            new ContentfulNews { Slug = "news-1", Title = "First News" },
            new ContentfulNews { Slug = "news-2", Title = "Second News" }
        };

        ContentfulCollection<ContentfulNews> collection = new()
        {
            Items = new List<ContentfulNews>()
        };

        _cacheWrapper
            .Setup(cacheWrapper => cacheWrapper.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(expectedNewsList);

        // Act
        HttpResponse response = await _repository.GetNews("news-of-the-century", "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No news found for 'news-of-the-century'", response.Error);
    }

    [Fact]
    public void Get_ShouldReturnAllNewsItems()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        ContentfulNewsRoom contentfulNewsRoom = new() { Title = "test" };
        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                contentfulNews,
                contentfulNews2
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(contentfulNewsRoom);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-categories", It.IsAny<Func<Task<List<string>>>>(), 60))
            .ReturnsAsync(new List<string> { "Benefits", "foo", "Council leader" });

        _videoRepository
            .Setup(videoRepo => videoRepo.Process(It.IsAny<string>()))
            .Returns("The news");

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, null, null));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Newsroom newsroom = response.Get<Newsroom>();
        Assert.Single(newsroom.Alerts);
        Assert.Equal(_alerts[0].Title, newsroom.Alerts[0].Title);
        Assert.True(newsroom.EmailAlerts);
        Assert.Equal("test-id", newsroom.EmailAlertsTopicId);
        Assert.Equal(3, newsroom.Categories.Count);
        Assert.Equal("Benefits", newsroom.Categories.First());
        Assert.Equal("Council leader", newsroom.Categories.Last());
        Assert.Single(newsroom.Dates);
        Assert.Equal(8, newsroom.Dates[0].Month);
        Assert.Equal(2016, newsroom.Dates[0].Year);
        Assert.Equal(2, newsroom.News.Count);

        News firstNews = newsroom.News.First();
        Assert.Equal("This is the news", firstNews.Title);
        Assert.Equal("The body", firstNews.Body);
        Assert.Equal("news-of-the-century", firstNews.Slug);
        Assert.Equal("Read more", firstNews.Teaser);
        Assert.Equal(_sunriseDate, firstNews.SunriseDate);
        Assert.Equal(_sunsetDate, firstNews.SunsetDate);
        Assert.Equal(_updatedAt, firstNews.UpdatedAt);
        Assert.Equal("image.jpg", firstNews.Image);
        Assert.Equal("thumbnail.jpg", firstNews.ThumbnailImage);
        Assert.Equal(_alerts, firstNews.Alerts);
        Assert.Equal(2, firstNews.Categories.Count);
        Assert.Equal("news-category-1", firstNews.Categories[0]);
        Assert.Equal("news-category-2", firstNews.Categories[1]);
    }

    [Fact]
    public void Get_ShouldReturnAllNewsItemsWhenNoNewsroomIsPresent()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(new(), true, string.Empty, null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                contentfulNews,
                contentfulNews2
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _videoRepository
            .Setup(videoRepo => videoRepo.Process(It.IsAny<string>()))
            .Returns("The news");

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get(null, null, null, null, null));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Newsroom newsroom = response.Get<Newsroom>();
        Assert.Empty(newsroom.Alerts);
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal("This is the news", newsroom.News.First().Title);
        Assert.Equal("The body", newsroom.News.First().Body);
        Assert.Equal("news-of-the-century", newsroom.News.First().Slug);
        Assert.Equal("Read more", newsroom.News.First().Teaser);
        Assert.Equal(_sunriseDate, newsroom.News.First().SunriseDate);
        Assert.Equal(_sunsetDate, newsroom.News.First().SunsetDate);
        Assert.Equal(_updatedAt, newsroom.News.First().UpdatedAt);
        Assert.Equal("image.jpg", newsroom.News.First().Image);
        Assert.Equal("thumbnail.jpg", newsroom.News.First().ThumbnailImage);
        Assert.Equal(_alerts, newsroom.News.First().Alerts);
        Assert.Single(newsroom.Dates);
        Assert.Equal(8, newsroom.Dates[0].Month);
        Assert.Equal(2016, newsroom.Dates[0].Year);
    }

    [Fact]
    public void Get_ShouldReturnListOfNewsForTag()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);
        
        News news = CreateTestNews("This is the news", "news-of-the-century");
        news.Tags = new() { "Events" };

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                contentfulNews,
                contentfulNews2
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>()
                                                                                                .ContentTypeIs("news")
                                                                                                .Include(1)
                                                                                                .FieldEquals("fields.tags[in]", "Events")
                                                                                                .Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get("Events", null, null, null, null));
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(newsroom.Alerts);
        Assert.True(newsroom.EmailAlerts);
        Assert.Equal("test-id", newsroom.EmailAlertsTopicId);
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal("This is the news", newsroom.News.First().Title);
        Assert.Equal("news-of-the-century", newsroom.News.First().Slug);
    }

    [Fact]
    public void Get_ShouldReturnListOfNewsForCategory()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                contentfulNews,
                contentfulNews2
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get(null, "news-category-1", null, null, null));
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(newsroom.Alerts);
        Assert.True(newsroom.EmailAlerts);
        Assert.Equal("test-id", newsroom.EmailAlertsTopicId);
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal("This is the news", newsroom.News.First().Title);
        Assert.Equal("news-of-the-century", newsroom.News.First().Slug);
    }

    [Fact]
    public void Get_ShouldReturnListOfNewsForCategoryAndTag()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);
        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");
        news.Tags = new() { "Events" };

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                contentfulNews,
                contentfulNews2
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>()
                                                                                                        .ContentTypeIs("news")
                                                                                                        .Include(1)
                                                                                                        .FieldEquals("fields.tags[in]", "Events")
                                                                                                        .Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get("Events", "news-category-1", null, null, null));
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(newsroom.Alerts);
        Assert.True(newsroom.EmailAlerts);
        Assert.Equal("test-id", newsroom.EmailAlertsTopicId);
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal("This is the news", newsroom.News.First().Title);
        Assert.Equal("news-of-the-century", newsroom.News.First().Slug);
    }

    [Fact]
    public async Task Get_ShouldReturnListOfNewsForDateRange()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 09, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is within the date range", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                new ContentfulNewsBuilder()
                    .Title("This is within the date range")
                    .Slug("news-of-the-century")
                    .Teaser("Read more")
                    .SunriseDate(new(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc))
                    .SunsetDate(new(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc))
                    .Build(),
                contentfulNews2
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = await _repository.Get(null, null, new DateTime(2016, 08, 01), new DateTime(2016, 08, 31), null);
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(2, newsroom.News.Count);
        Assert.Equal("This is within the date range", newsroom.News.First().Title);
    }

    [Fact]
    public void Get_ShouldReturnNoListOfNewsForDateRange()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 09, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);
        
        News news = CreateTestNews("This is within the date Range", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                new ContentfulNewsBuilder()
                    .Title("This is within the date Range")
                    .Slug("news-of-the-century")
                    .Teaser("Read more")
                    .SunriseDate(new(2016, 08, 24, 23, 30, 0, DateTimeKind.Utc))
                    .SunsetDate(new(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc))
                    .Build(),
                contentfulNews2
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get(null,
                                                                        null,
                                                                        new DateTime(2017, 08, 01),
                                                                        new DateTime(2017, 08, 31),
                                                                        null));
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Empty(newsroom.News);
    }

    [Fact]
    public async Task Get_ShouldReturnListOfFilterDatesForAllNewsThatIsCurrentOrPast()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                contentfulNews,
                contentfulNews2
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = await _repository.Get(null, null, null, null, null);
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Single(newsroom.Dates);
        Assert.Equal(new(2016, 08, 01), newsroom.Dates.First().Date);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFoundForTagAndCategory()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                contentfulNews,
                contentfulNews2
            }
        };

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = await _repository.Get("NotFound", "NotFound", null, null, null);
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Empty(newsroom.News);
        Assert.Null(newsroom.Categories);
    }

    [Fact]
    public async Task Get_ShouldReturnNewsItemsWithExactMatchingesForTagsWithoutHash()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");
        news.Tags = new() { "testTag" };
        
        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        contentfulNews.Tags = new() { "testTag", "bar" };
        contentfulNews3.Tags = new() { "testTag", "foo" };

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                contentfulNews,
                contentfulNews2
            }
        };

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = await _repository.Get("testTag", null, new DateTime(2016, 08, 01), new DateTime(2016, 08, 31), "tagId");
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(2, newsroom.News.Count);
        Assert.Contains(newsroom.News.First().Tags, t => t.Equals("testTag"));
    }
    
     [Fact]
    public void Get_ShouldReturnNewsItemsWithTagsContainingMatchingTagsWithHash()
    {
        // Arrange
        const string expectedTagQueryValue = "testTag";
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);
        
        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");
        news.Tags = new() { "testTag" };

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        contentfulNews.Tags = new() { "#testTag", "foo" };
        contentfulNews3.Tags = new() { "#testTag", "foo" };
        contentfulNews2.Tags = new() { "sports" };

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews3,
                contentfulNews,
                contentfulNews2
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>()
                                                                                        .ContentTypeIs("news")
                                                                                        .Include(1)
                                                                                        .FieldEquals("fields.tags[match]", "testTag")
                                                                                        .Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);


        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get("testTag", null, null, null, "tagId"));
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(2, newsroom.News.Count);
        Assert.Contains(newsroom.News[1].Tags, tag => tag.Equals(expectedTagQueryValue));
    }

    [Fact]
    public async Task GetNewsByLimit_ShouldReturnTopNewsItems()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2020, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);
        
        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns<ContentfulNews>(entry => CreateTestNews(entry.Title, entry.Slug, entry.SunriseDate, entry.SunsetDate));

        ContentfulCollection<ContentfulNews> newsListCollection = new();

        contentfulNews.SunriseDate = new(2018, 08, 24, 23, 30, 0, DateTimeKind.Unspecified);
        contentfulNews.SunsetDate = new(2025, 08, 23, 23, 0, 0, DateTimeKind.Utc);

        newsListCollection.Items = new List<ContentfulNews>
        {
            contentfulNews2,
            contentfulNews3,
            contentfulNews
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _videoRepository
            .Setup(videoRepo => videoRepo.Process(It.IsAny<string>()))
            .Returns("The news");

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = await _repository.GetNewsByLimit(1, "tagId");

        // Arrange
        List<News> newsList = response.Get<List<News>>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(newsList);
        Assert.Equal(contentfulNews.Title, newsList.First().Title);
        Assert.Equal(contentfulNews.Slug, newsList.First().Slug);
        Assert.Equal(contentfulNews.Teaser, newsList.First().Teaser);
        Assert.Equal(contentfulNews.SunriseDate, newsList.First().SunriseDate);
        Assert.Equal(contentfulNews.SunsetDate, newsList.First().SunsetDate);
    }

    [Fact]
    public async Task GetNewsByLimit_ShouldReturnTopTwoNewsItems()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                contentfulNews2,
                contentfulNews3,
                contentfulNews
            }
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _videoRepository
            .Setup(videoRepo => videoRepo.Process(It.IsAny<string>()))
            .Returns("The news");

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(new ContentfulNewsRoom { Title = "test" });

        // Act
        HttpResponse response = await _repository.GetNewsByLimit(2, "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<News> newsList = response.Get<List<News>>();

        Assert.Equal(2, newsList.Count);
        Assert.Equal("This is the news", newsList.First().Title);
        Assert.Equal("The body", newsList.First().Body);
        Assert.Equal("news-of-the-century", newsList.First().Slug);
        Assert.Equal("Read more", newsList.First().Teaser);
        Assert.Equal(_sunriseDate, newsList.First().SunriseDate);
        Assert.Equal(_sunsetDate, newsList.First().SunsetDate);
        Assert.Equal(_updatedAt, newsList.First().UpdatedAt);
        Assert.Equal("image.jpg", newsList.First().Image);
        Assert.Equal("thumbnail.jpg", newsList.First().ThumbnailImage);
        Assert.Equal(_alerts, newsList.First().Alerts);
    }

    [Fact]
    public void GetNews_ShouldReturnNotFoundIfNewsHasSunriseDateAfterToday()
    {
        // Arrange
        const string slug = "news-with-sunrise-date-in-future";
        DateTime futureSunRiseDate = DateTime.Now.AddDays(10);

        ContentfulNews newsWithSunriseDateInFuture = new ContentfulNewsBuilder().SunriseDate(futureSunRiseDate).Slug(slug).Build();
        ContentfulCollection<ContentfulNews> collection = new()
        {
            Items = new List<ContentfulNews> { newsWithSunriseDateInFuture }
        };

        string simpleNewsQuery = new QueryBuilder<ContentfulNews>()
            .ContentTypeIs("news")
            .FieldEquals("fields.slug", slug)
            .Include(1)
            .Build();

        _videoRepository
            .Setup(videoRepo => videoRepo.Process(It.IsAny<string>()))
            .Returns(newsWithSunriseDateInFuture.Body);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(collection.Items.ToList());

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetNews(slug, "tagId"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetNews_ShouldReturnSuccessIfNewsArticleSunsetDateIsInThePast()
    {
        // Arrange
        ContentfulNews newsWithSunsetDateInPast = new ContentfulNewsBuilder()
            .SunsetDate(DateTime.Now.AddDays(-10))
            .SunriseDate(DateTime.Now.AddDays(-20))
            .Slug("news-with-sunrise-date-in-future")
            .Build();

        ContentfulCollection<ContentfulNews> collection = new()
        {
            Items = new List<ContentfulNews> { newsWithSunsetDateInPast }
        };

        string simpleNewsQuery = new QueryBuilder<ContentfulNews>()
            .ContentTypeIs("news")
            .FieldEquals("fields.slug", "news-with-sunrise-date-in-future")
            .Include(1)
            .Build();

        _videoRepository
            .Setup(videoRepo => videoRepo.Process(It.IsAny<string>()))
            .Returns(newsWithSunsetDateInPast.Body);

        _cacheWrapper
            .Setup(wrapper => wrapper.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(collection.Items.ToList());

        // Act
        HttpResponse response = await _repository.GetNews("news-with-sunrise-date-in-future", "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(Skip = "This test is not working as expected. It needs to be fixed.")]
    public async Task Get_ShouldReturnNewsOrderedBySunriseDate()
    {
        // Arrange
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 5));

        ContentfulNewsRoom contentfulNewsRoom = new() { Title = "test" };
        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(newsRoomFactory => newsRoomFactory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(news);

        ContentfulCollection<ContentfulNews> newsListCollection = new()
        {
            Items = new List<ContentfulNews>
            {
                new ContentfulNewsBuilder()
                    .Title("Oldest news article")
                    .Slug("old-news")
                    .Teaser("This is news")
                    .SunriseDate(new(2016, 01, 01, 23, 0, 0, DateTimeKind.Utc))
                    .SunsetDate(new(2017, 11, 22, 23, 0, 0, DateTimeKind.Utc))
                    .Build(),
                new ContentfulNewsBuilder()
                    .Title("middle news article")
                    .Slug("middle-news")
                    .Teaser("This is news")
                    .SunriseDate(new(2016, 01, 02, 23, 30, 0, DateTimeKind.Utc))
                    .SunsetDate(new(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc))
                    .Build(),
                new ContentfulNewsBuilder()
                    .Title("newsest news article")
                    .Slug("new-news")
                    .Teaser("This is news")
                    .SunriseDate(new(2016, 01, 03, 23, 30, 0, DateTimeKind.Utc))
                    .SunsetDate(new(2016, 08, 23, 23, 0, 0, DateTimeKind.Utc))
                    .Build()
            }
        };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newsListCollection);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsListCollection.Items.ToList());

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(contentfulNewsRoom);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-categories", It.IsAny<Func<Task<List<string>>>>(), 60))
            .ReturnsAsync(new List<string> { "Benefits", "foo", "Council leader" });

        _videoRepository
            .Setup(videoRepo => videoRepo.Process(It.IsAny<string>()))
            .Returns("The news");

        // Act
        HttpResponse response = await _repository.Get(null, null, null, null, null);

        // Assert
        Newsroom newsroom = response.Get<Newsroom>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("new-news", newsroom.News[0].Slug);
        Assert.Equal("middle-news", newsroom.News[1].Slug);
        Assert.Equal("old-news", newsroom.News[2].Slug);
    }

    [Fact]
    public async Task GetLatestNewsByTag_ShouldReturnNews_WhenValidNewsIsFound()
    {
        // Arrange
        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = contentfulNewsList });

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(expectedNews);

        // Act
        List<News> result = await _repository.GetLatestNewsByTag("tech", "tagId", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedNews, result.FirstOrDefault());
    }

    [Fact]
    public async Task GetLatestNewsByTag_ShouldReturnNull_WhenNoValidNewsIsFound()
    {
        // Arrange
        List<ContentfulNews> newsList = new();

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = newsList });

        // Act
        List<News> result = await _repository.GetLatestNewsByTag("tech", "tagId");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLatestNewsByCategory_ShouldReturnNews_WhenValidNewsIsFound()
    {
        // Arrange
        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = contentfulNewsList });

        _newsFactory
            .Setup(newsFactory => newsFactory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns(expectedNews);

        // Act
        List<News> result = await _repository.GetLatestNewsByCategory("sports", "tagId");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedNews, result.FirstOrDefault());
    }

    [Fact]
    public async Task GetLatestNewsByCategory_ShouldReturnNull_WhenDateIsOutOfRange()
    {
        // Arrange
        List<ContentfulNews> newsList = new()
        {
            new()
            {
                Categories = new() { "sports" },
                SunriseDate = DateTime.Now.AddDays(-10),
                SunsetDate = DateTime.Now.AddDays(-5)
            }
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = newsList });

        // Act
        List<News> result = await _repository.GetLatestNewsByCategory("sports", "tagId");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetArchivedNews_ShouldReturnNotFound_WhenNoNewsIsFound()
    {
        // Arrange
        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = contentfulNewsList });

        // Act
        HttpResponse response = await _repository.GetArchivedNews(null, null, null, null, null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetArchivedNews_ShouldReturnOk_WhenNewsIsFound()
    {
        // Arrange
        SetupMocks();

        // Act
        HttpResponse response = await _repository.GetArchivedNews(null, null, null, null, null);
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(newsroom.News);
        Assert.Equal("news-of-the-century", newsroom.News.First().Slug);
        Assert.Equal("another-news-article", newsroom.News.Last().Slug);
    }

    [Theory]
    [InlineData(null, null, "2016-08-01", "2016-08-31", "news-of-the-century")]
    [InlineData(null, "Benefits", "2016-06-01", "2016-09-01", "another-news-article")]
    [InlineData("sports", null, "2016-06-01", "2016-09-01", "news-of-the-century")]
    public async Task GetArchivedNews_ShouldReturnOk_WhenFiltersMatch(string tag, string category, string startDateString, string endDateString, string expectedSlug)
    {
        // Arrange
        SetupMocks();
        DateTime startDate = DateTime.Parse(startDateString);
        DateTime endDate = DateTime.Parse(endDateString);

        // Act
        HttpResponse response = await _repository.GetArchivedNews(tag, category, startDate, endDate, "tagId");
        Newsroom newsroom = response.Get<Newsroom>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(newsroom.News);
        Assert.Equal(expectedSlug, newsroom.News.First().Slug);
    }

    private void SetupMocks()
    {
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(DateTime.Now);

        ContentfulNewsRoom contentfulNewsRoom = new() { Title = "test" };
        Newsroom newsRoom = new(_alerts, true, "test-id", null);

        _newsRoomContentfulFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulNewsRoom>()))
            .Returns(newsRoom);

        News news = CreateTestNews("This is the news", "news-of-the-century");

        _newsFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulNews>()))
            .Returns<ContentfulNews>(contentfulNews => new News(
                contentfulNews.Title,
                contentfulNews.Slug,
                contentfulNews.Teaser,
                "image.jpg",
                "thumbnail.jpg",
                "hero caption image",
                "The news",
                contentfulNews.SunriseDate,
                contentfulNews.SunsetDate,
                _updatedAt,
                _alerts,
                new() { "tag1", "tag2" },
                contentfulNews.Categories,
                null,
                null,
                string.Empty,
                null,
                null,
                string.Empty
            ));

        contentfulNews.Tags = new() { "sports" };
        List<ContentfulNews> newsList = new()
        {
            contentfulNews3,
            contentfulNews,
            contentfulNews2
        };

        ContentfulCollection<ContentfulNews> collection = new() { Items = newsList };

        _client
            .Setup(client => client.GetEntries<ContentfulNews>(It.Is<string>(query => query.Contains(new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1).Build())),
                                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-all", It.IsAny<Func<Task<IList<ContentfulNews>>>>(), 60))
            .ReturnsAsync(newsList);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("newsroom", It.IsAny<Func<Task<ContentfulNewsRoom>>>(), 60))
            .ReturnsAsync(contentfulNewsRoom);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync("news-categories", It.IsAny<Func<Task<List<string>>>>(), 60))
            .ReturnsAsync(new List<string> { "Benefits", "foo", "Council leader" });

        _videoRepository
            .Setup(repo => repo.Process(It.IsAny<string>()))
            .Returns("The news");
    }

    private News CreateTestNews(string title, string slug, DateTime? sunriseDate = null, DateTime? sunsetDate = null) =>
        new(title,
            slug,
            "Read more",
            "image.jpg",
            "thumbnail.jpg",
            "hero caption",
            "The body",
            sunriseDate ?? _sunriseDate,
            sunsetDate ?? _sunsetDate,
            _updatedAt,
            _alerts,
            new(),
            _newsCategories,
            null,
            null,
            string.Empty,
            null,
            null,
            string.Empty);
}