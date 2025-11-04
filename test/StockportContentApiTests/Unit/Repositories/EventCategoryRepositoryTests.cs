namespace StockportContentApiTests.Unit.Repositories;

public class EventCategoryRepositoryTests
{
    private readonly EventCategoryRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>> _eventCategoryFactory = new();
    private readonly Mock<ICache> _cacheWrapper = new();
    private readonly Mock<IConfiguration> _configuration = new();

    public EventCategoryRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        CacheKeyConfig _cacheKeyconfig = new CacheKeyConfig("test")
            .Add("TEST_EventsCacheKey", "testEventsCacheKey")
            .Add("TEST_NewsCacheKey", "testNewsCacheKey")
            .Build();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(config))
            .Returns(_contentfulClient.Object);

        _configuration
            .Setup(configuration => configuration["redisExpiryTimes:Events"])
            .Returns("60");

        _repository = new EventCategoryRepository(config,
                                                _cacheKeyconfig,
                                                _eventCategoryFactory.Object,
                                                contentfulClientManager.Object,
                                                _cacheWrapper.Object,
                                                _configuration.Object);
    }

    [Fact]
    public async Task ItGetsEventCategories()
    {
        // Arrange
        EventCategory rawEventCategory = new("name", "slug", "icon", string.Empty);
        _cacheWrapper
            .Setup(cacheWrapper => cacheWrapper.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-categories-content-type")),
                                                                            It.IsAny<Func<Task<List<EventCategory>>>>(),
                                                                            It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<EventCategory> { rawEventCategory });

        // Act
        HttpResponse response = await _repository.GetEventCategories("tagId");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void ShouldReturnNotFoundIfNoEventCategoryFound()
    {
        // Arrange
        ContentfulCollection<ContentfulEventCategory> collection = new()
        {
            Items = new List<ContentfulEventCategory>()
        };

        _cacheWrapper
            .Setup(cacheWrapper => cacheWrapper.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-categories-content-type")),
                                                                            It.IsAny<Func<Task<List<EventCategory>>>>(),
                                                                            It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<EventCategory>());

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetEventCategories("tagId"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}