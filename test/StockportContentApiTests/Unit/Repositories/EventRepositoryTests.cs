namespace StockportContentApiTests.Unit.Repositories;

public class EventRepositoryTests
{
    private readonly Mock<IContentfulClientManager> _contentfulClientManager = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory = new();
    private readonly Mock<ICache> _cacheWrapper = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly EventRepository _repository;

    public EventRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        CacheKeyConfig cacheKeyConfig = new CacheKeyConfig("test")
            .Add("TEST_EventsCacheKey", "testEventsCacheKey")
            .Add("TEST_NewsCacheKey", "testNewsCacheKey")
            .Build();

        _timeProvider
            .Setup(provider => provider.Now())
            .Returns(new DateTime(2017, 01, 01));

        _contentfulClientManager
            .Setup(manager => manager.GetClient(config))
            .Returns(_contentfulClient.Object);

        _configuration
            .Setup(configuration => configuration["redisExpiryTimes:Articles"])
            .Returns("60");

        _configuration
            .Setup(configuration => configuration["redisExpiryTimes:Events"])
            .Returns("60");

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new()
                {
                    EventDate = DateTime.Today.AddDays(1),
                    StartTime = "07:00:00",
                    Title = "Title",
                    Slug = "Slug"
                }
            });

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<ContentfulCollection<ContentfulEventCategory>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEventCategory>
            {
                Items = new List<ContentfulEventCategory>
                {
                    new()
                    {
                        Slug = "Slug"
                    }
                }
                
            });

        _eventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder().EventDate(DateTime.Today.AddDays(1)).Slug("Slug").Build);

        List<EventHomepageRow> eventHomepageRows = new()
        {
            new()
            {
                IsLatest = true
            }
        };

        _eventHomepageFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEventHomepage>()))
            .Returns(new EventHomepage(eventHomepageRows));

        _repository = new(config,
                        cacheKeyConfig,
                        _contentfulClientManager.Object,
                        _timeProvider.Object,
                        _eventFactory.Object,
                        _eventHomepageFactory.Object,
                        _cacheWrapper.Object,
                        _configuration.Object);
    }

    [Fact]
    public async Task GetEventHomepage_ShouldCallClient()
    {
        // Arrange
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEventHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEventHomepage>
            {
                Items = new List<ContentfulEventHomepage> { new() }
            });

        // Act
        await _repository.GetEventHomepage("tagId");

        // Assert
        _contentfulClient.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEventHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetEventHomepage_ShouldCallEventHomepageFactory()
    {
        // Arrange
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEventHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEventHomepage>
            {
                Items = new List<ContentfulEventHomepage> { new() }
            });

        // Act
        await _repository.GetEventHomepage("tagId");

        // Assert
        _eventHomepageFactory.Verify(factory => factory.ToModel(It.IsAny<ContentfulEventHomepage>()), Times.Once);
    }

    [Fact]
    public async Task GetEventHomepage_ShouldCallCache()
    {
        // Arrange
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEventHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEventHomepage>
            {
                Items = new List<ContentfulEventHomepage> { new() }
            });

        // Act
        await _repository.GetEventHomepage("tagId");

        // Assert
        _cacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetContentfulEventCategories_ShouldCallCache()
    {
        // Act
        await _repository.GetContentfulEventCategories("tagId");

        // Assert
        _cacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<ContentfulCollection<ContentfulEventCategory>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetContentfulEventCategories_ShouldReturnResultsIfNotEmpty()
    {
        // Act
        ContentfulCollection<ContentfulEventCategory> results = await _repository.GetContentfulEventCategories("tagId");

        // Assert
        Assert.Single(results);
    }

    [Fact]
    public async Task GetEvent_ShouldCallCache()
    {
        // Act
        await _repository.GetEvent("Slug", DateTime.Today.AddDays(1), "tagId");

        // Assert
        _cacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetEvent_ShouldReturnSuccessful()
    {
        // Act
        HttpResponse result = await _repository.GetEvent("Slug", DateTime.Today.AddDays(1), "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task GetEvent_ShouldReturnNotFound()
    {
        // Arrange
        _eventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder().EventDate(DateTime.Today.AddDays(1)).Slug("None").Build);

        // Act
        HttpResponse result = await _repository.GetEvent("Slug", DateTime.Today.AddDays(1), "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldCallCache()
    {
        // Act
        await _repository.Get(DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), "Category", 0, true, "Tag", "Price", 0, 0, false, "tagId");

        // Assert
        _cacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound()
    {
        // Arrange
        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>());

        _eventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder().EventDate(DateTime.Today.AddDays(1)).Slug("None").Build);

        // Act
        HttpResponse result = await _repository.Get(DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), "Category", 0, true, string.Empty, string.Empty, 0, 0, false, "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Theory]
    [InlineData(true, true, "Slug", "", "paid,free", true, 0, true)]
    [InlineData(false, true, "", null, "free,paid", null, 0, false)]
    [InlineData(true, false, "Slug", "", "free", false, 1, null)]
    [InlineData(true, false, "Slug", "tag", "paid", false, 1, false)]
    [InlineData(true, false, "Slug", "tag", null, false, 1, false)]
    [InlineData(true, false, "", "", null, false, 0, true)]
    public async Task Get_ShouldReturnSuccessful(bool dateFromNull, bool dateToNull, string category, string tag, string price, bool? displayFeatured, int limit, bool? free)
    {
        // Arrange
        _eventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Slug("Slug")
                .Fee(price)
                .Free(!free)
                .EventCategories(new List<EventCategory> { new("Slug", "Slug", "icon", string.Empty) })
                .Build);

        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new ContentfulEventBuilder().Build(),
                new ContentfulEventBuilder().Build()
            });

        DateTime? dateFrom = dateFromNull ? null : DateTime.Today.AddDays(1);
        DateTime? dateTo = dateToNull ? null : DateTime.Today.AddDays(1);

        // Act
        HttpResponse result = await _repository.Get(dateFrom, dateTo, category, limit, displayFeatured, tag, price, 0, 0, free, "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Theory]
    [InlineData("art", "art", "Arty", true)]
    [InlineData("art", "art", "Arty", false)]
    [InlineData("art", "arty", "art", true)]
    [InlineData("art", "arty", "art", false)]
    [InlineData("", "arty", "art", false)]
    public async Task GetEventsByCategory_ShouldCallCache(string category, string slug, string name, bool onlyNextOccurrence)
    {
        // Arrange
        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new ContentfulEventBuilder().Build()
            });

        _eventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Slug("Slug")
                .EventCategories(new List<EventCategory> { new(name, slug, "icon", string.Empty) })
                .Build);

        // Act
        await _repository.GetEventsByCategory(category, onlyNextOccurrence, "tagId");

        // Assert
        _cacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Theory]
    [InlineData("art", "art", true)]
    [InlineData("art", "art", false)]
    [InlineData("", "", true)]
    [InlineData("", "",  false)]
    public async Task GetEventsByTag_ShouldCallCache(string tagRequested, string tagSet, bool onlyNextOccurrence)
    {
        // Arrange
        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new ContentfulEventBuilder().Build()
            });

        _eventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Slug("Slug")
                .Tags([tagSet])
                .Build);

        // Act
        await _repository.GetEventsByTag(tagRequested, onlyNextOccurrence, "tagId");

        // Assert
        _cacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetAllEvents_ShouldReturnResults()
    {
        // Arrange
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEvent>
            {
                Items = new List<ContentfulEvent> { new() }
            });

        // Act
        IList<ContentfulEvent> result = await _repository.GetAllEvents("tagId");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAllEvents_ShouldReturnNull()
    {
        // Arrange
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEvent>
            {
                Items = new List<ContentfulEvent>()
            });

        // Act
        IList<ContentfulEvent> result = await _repository.GetAllEvents("tagId");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLinkedEvents_ShouldCallCache()
    {
        // Arrange
        _cacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new ContentfulEventBuilder().Build()
            });

        _eventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Slug("Slug")
                .Build);

        // Act
        await _repository.GetLinkedEvents<ContentfulEvent>("Slug", "tagId");

        // Assert
        _cacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public void GetRelatedEvents_ShouldCallFactory_Categories()
    {
        // Arrange
        _eventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Build);

        // Act
        _repository.GetRelatedEvents(new List<ContentfulEvent> { new() }, "eventSlug", ["Category name"], ["category-slug"], []);

        // Assert
        _eventFactory.Verify(factory => factory.ToModel(It.IsAny<ContentfulEvent>()), Times.Once);
    }

    [Fact]
    public void GetRelatedEvents_ShouldCallFactory_Tags()
    {
        // Arrange
        _eventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Tags(["tag"])
                .Build);

        // Act
        _repository.GetRelatedEvents(new List<ContentfulEvent> { new() }, "eventSlug", [], [], ["tag"]);

        // Assert
        _eventFactory.Verify(factory => factory.ToModel(It.IsAny<ContentfulEvent>()), Times.Once);
    }

    [Fact]
    public async Task GetContentfulEvent_ShouldCallClient()
    {
        // Arrange
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEvent>
            {
                Items = new List<ContentfulEvent> { new() }
            });

        // Act
        await _repository.GetContentfulEvent("slug", "tagId");

        // Assert
        _contentfulClient.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}