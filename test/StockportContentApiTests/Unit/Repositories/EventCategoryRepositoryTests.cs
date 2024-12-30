namespace StockportContentApiTests.Unit.Repositories;

public class EventCategoryRepositoryTests
{
    private readonly EventCategoryRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>> _contentfulEventCategoryFactory;
    private readonly Mock<ICache> _cacheWrapper;
    private readonly Mock<IConfiguration> _configuration;
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
        _contentfulClient = new Mock<IContentfulClient>();
        _contentfulEventCategoryFactory = new Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
        _cacheWrapper = new Mock<ICache>();
        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");
        _repository = new EventCategoryRepository(config, _cacheKeyconfig, _contentfulEventCategoryFactory.Object, contentfulClientManager.Object, _cacheWrapper.Object, _configuration.Object);
    }

    [Fact]
    public void ItGetsEventCategories()
    {
        // Arrange
        EventCategory rawEventCategory = new("name", "slug", "icon", string.Empty);
        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-categories-content-type")), It.IsAny<Func<Task<List<EventCategory>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(new List<EventCategory> { rawEventCategory });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetEventCategories());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void ShouldReturnNotFoundIfNoEventCategoryFound()
    {
        ContentfulCollection<ContentfulEventCategory> collection = new()
        {
            Items = new List<ContentfulEventCategory>()
        };

        _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-categories-content-type")), It.IsAny<Func<Task<List<EventCategory>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(new List<EventCategory>());

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetEventCategories());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
