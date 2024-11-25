using System.Xml.Linq;
using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Repositories;

public class EventRepositoryTests
{
    private readonly Mock<IContentfulClientManager> _contentfulClientManager = new();
    private readonly Mock<ITimeProvider> _mockTimeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _mockEventFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _mockEventHomepageFactory = new();
    private readonly Mock<ICache> _mockCacheWrapper = new();
    private readonly Mock<IConfiguration> _configuration = new();

    private readonly Mock<IContentfulClient> _mockContentfulClient = new();
    
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

        _mockTimeProvider
            .Setup(provider => provider.Now())
            .Returns(new DateTime(2017, 01, 01));

        _contentfulClientManager
            .Setup(manager => manager.GetClient(config))
            .Returns(_mockContentfulClient.Object);

        _configuration
            .Setup(configuration => configuration["redisExpiryTimes:Articles"])
            .Returns("60");

        _configuration
            .Setup(configuration => configuration["redisExpiryTimes:Events"])
            .Returns("60");

        _mockCacheWrapper
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

        _mockCacheWrapper
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

        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder().EventDate(DateTime.Today.AddDays(1)).Slug("Slug").Build);

        List<EventHomepageRow> eventHomepageRows = new()
        {
            new()
            {
                IsLatest = true
            }
        };

        _mockEventHomepageFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEventHomepage>()))
            .Returns(new EventHomepage(eventHomepageRows));

        _repository = new(config,
                          cacheKeyConfig,
                          _contentfulClientManager.Object,
                          _mockTimeProvider.Object,
                          _mockEventFactory.Object,
                          _mockEventHomepageFactory.Object,
                          _mockCacheWrapper.Object,
                          _configuration.Object);
    }

    [Fact]
    public async Task GetEventHomepage_ShouldCallClient()
    {
        // Arrange
        _mockContentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEventHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEventHomepage>
            {
                Items = new List<ContentfulEventHomepage> { new() }
            });

        // Act
        await _repository.GetEventHomepage();

        // Assert
        _mockContentfulClient.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEventHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetEventHomepage_ShouldCallEventHomepageFactory()
    {
        // Arrange
        _mockContentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEventHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEventHomepage>
            {
                Items = new List<ContentfulEventHomepage> { new() }
            });

        // Act
        await _repository.GetEventHomepage();

        // Assert
        _mockEventHomepageFactory.Verify(factory => factory.ToModel(It.IsAny<ContentfulEventHomepage>()), Times.Once);
    }

    [Fact]
    public async Task GetEventHomepage_ShouldCallCache()
    {
        // Arrange
        _mockContentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEventHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEventHomepage>
            {
                Items = new List<ContentfulEventHomepage> { new() }
            });

        // Act
        await _repository.GetEventHomepage();

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetAllEventsForAGroup_ShouldCallCache()
    {
        // Act
        await _repository.GetAllEventsForAGroup("Slug");

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetContentfulEventCategories_ShouldCallCache()
    {
        // Act
        await _repository.GetContentfulEventCategories();

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<ContentfulCollection<ContentfulEventCategory>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetContentfulEventCategories_ShouldReturnResultsIfNotEmpty()
    {
        // Act
        ContentfulCollection<ContentfulEventCategory> results = await _repository.GetContentfulEventCategories();

        // Assert
        Assert.Single(results);
    }

    [Fact]
    public async Task GetEvent_ShouldCallCache()
    {
        // Act
        await _repository.GetEvent("Slug", DateTime.Today.AddDays(1));

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetEvent_ShouldReturnSuccessful()
    {
        // Act
        HttpResponse result = await _repository.GetEvent("Slug", DateTime.Today.AddDays(1));

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task GetEvent_ShouldReturnNotFound()
    {
        // Arrange
        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder().EventDate(DateTime.Today.AddDays(1)).Slug("None").Build);

        // Act
        HttpResponse result = await _repository.GetEvent("Slug", DateTime.Today.AddDays(1));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldCallCache()
    {
        // Act
        await _repository.Get(DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), "Category", 0, true, "Tag", "Price", 0, 0);

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound()
    {
        // Arrange
        _mockCacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>());

        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder().EventDate(DateTime.Today.AddDays(1)).Slug("None").Build);

        // Act
        HttpResponse result = await _repository.Get(DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), "Category", 0, true, string.Empty, string.Empty, 0, 0);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Theory]
    [InlineData(true, true, "Slug", "", "paid,free", true, 0)]
    [InlineData(false, true, "", null, "free,paid", null, 0)]
    [InlineData(true, false, "Slug", "", "free", false, 1)]
    [InlineData(true, false, "Slug", "tag", "paid", false, 1)]
    [InlineData(true, false, "Slug", "tag", null, false, 1)]
    public async Task Get_ShouldReturnSuccessful(bool dateFromNull, bool dateToNull, string category, string tag, string price, bool? displayFeatured, int limit)
    {
        // Arrange
        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Slug("Slug")
                .Fee(price)
                .EventCategories(new List<EventCategory> { new("Slug", "Slug", "icon") })
                .Build);

        _mockCacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new()
                {
                    EventDate = DateTime.Today.AddDays(1),
                    StartTime = "07:00:00",
                    Title = "Title",
                    Slug = "Slug",
                    Tags = ["Tag"],
                    Free = true,
                    EventCategories = new List<ContentfulEventCategory>
                    {
                        new()
                        {
                            Slug = "Slug",
                            Name = "Slug"
                        }
                    }
                }
            });

        DateTime? dateFrom = dateFromNull ? null : DateTime.Today.AddDays(1);
        DateTime? dateTo = dateToNull ? null : DateTime.Today.AddDays(1);

        // Act
        HttpResponse result = await _repository.Get(dateFrom, dateTo, category, limit, displayFeatured, tag, price, 0, 0);

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
        _mockCacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new()
                {
                    EventDate = DateTime.Today.AddDays(1),
                    StartTime = "07:00:00",
                    Title = "Title",
                    Slug = "Slug",
                    Tags = ["Tag"],
                    Free = true,
                    EventCategories = new List<ContentfulEventCategory>
                    {
                        new()
                        {
                            Slug = slug,
                            Name = name
                        }
                    }
                }
            });

        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Slug("Slug")
                .EventCategories(new List<EventCategory> { new(name, slug, "icon") })
                .Build);

        // Act
        await _repository.GetEventsByCategory(category, onlyNextOccurrence);

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Theory]
    [InlineData("art", "art", true)]
    [InlineData("art", "art", false)]
    [InlineData("", "", true)]
    [InlineData("", "",  false)]
    public async Task GetEventsByTag_ShouldCallCache(string tagRequested, string tagSet, bool onlyNextOccurrence)
    {
        // Arrange
        _mockCacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new()
                {
                    EventDate = DateTime.Today.AddDays(1),
                    StartTime = "07:00:00",
                    Title = "Title",
                    Slug = "Slug",
                    Tags = [tagSet],
                    Free = true
                }
            });

        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Slug("Slug")
                .Tags([tagSet])
                .Build);

        // Act
        await _repository.GetEventsByTag(tagRequested, onlyNextOccurrence);

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(null, 0)]
    public async Task GetFreeEvents_ShouldReturnEmptyIfNoFreeEvents(bool? free, int expectedResult)
    {
        // Arrange
        _mockCacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new()
                {
                    EventDate = DateTime.Today.AddDays(2),
                    StartTime = "12:00:00",
                    Title = "Title2",
                    Slug = "slug2",
                    Free = free
                }
            });

        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder().EventDate(DateTime.Today.AddDays(2)).Slug("slug2").Free(free).Build);

        // Act
        EventCalender result = await _repository.GetFreeEvents();

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
        Assert.Equal(expectedResult, result.Events.Count);
    }

    [Fact]
    public async Task GetFreeEvents_ShouldReturnOnlyFreeEvents()
    {
        // Act
        EventCalender result = await _repository.GetFreeEvents();

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
        Assert.Single(result.Events);
    }

    [Fact]
    public async Task GetAllEvents_ShouldReturnResults()
    {
        // Arrange
        _mockContentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEvent>
            {
                Items = new List<ContentfulEvent> { new() }
            });

        // Act
        var result = await _repository.GetAllEvents();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAllEvents_ShouldReturnNull()
    {
        // Arrange
        _mockContentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEvent>
            {
                Items = new List<ContentfulEvent>()
            });

        // Act
        var result = await _repository.GetAllEvents();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLinkedEvents_ShouldCallCache()
    {
        // Arrange
        _mockCacheWrapper
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<ContentfulEvent>
            {
                new()
                {
                    EventDate = DateTime.Today.AddDays(1),
                    StartTime = "07:00:00",
                    Title = "Title",
                    Slug = "Slug",
                    Free = true,
                    Group = new ContentfulGroup
                    {
                        Slug = "Slug"
                    }
                }
            });

        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Slug("Slug")
                .Group(new Group
                {
                    Slug = "Slug"
                })
                .Build);

        // Act
        await _repository.GetLinkedEvents<ContentfulEvent>("Slug");

        // Assert
        _mockCacheWrapper.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public void GetRelatedEvents_ShouldCallFactory_Categories()
    {
        // Arrange
        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Build);

        // Act
        _repository.GetRelatedEvents(new List<ContentfulEvent> { new() }, "eventSlug", ["Category name"], ["category-slug"], []);

        // Assert
        _mockEventFactory.Verify(factory => factory.ToModel(It.IsAny<ContentfulEvent>()), Times.Once);
    }

    [Fact]
    public void GetRelatedEvents_ShouldCallFactory_Tags()
    {
        // Arrange
        _mockEventFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder()
                .EventDate(DateTime.Today.AddDays(1))
                .Tags(["tag"])
                .Build);

        // Act
        _repository.GetRelatedEvents(new List<ContentfulEvent> { new() }, "eventSlug", [], [], ["tag"]);

        // Assert
        _mockEventFactory.Verify(factory => factory.ToModel(It.IsAny<ContentfulEvent>()), Times.Once);
    }

    [Fact]
    public async Task GetContentfulEvent_ShouldCallClient()
    {
        // Arrange
        _mockContentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulEvent>
            {
                Items = new List<ContentfulEvent> { new() }
            });

        // Act
        await _repository.GetContentfulEvent("slug");

        // Assert
        _mockContentfulClient.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}