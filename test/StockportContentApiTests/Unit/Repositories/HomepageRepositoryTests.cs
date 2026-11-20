namespace StockportContentApiTests.Unit.Repositories;

public class HomepageRepositoryTests
{
    private readonly HomepageRepository _repository;
    private readonly Mock<IContentfulFactory<ContentfulHomepage, Homepage>> _homepageFactory = new();
    private readonly Mock<IContentfulClient> _client = new();
    private readonly Mock<ICache> _cache = new();

    public HomepageRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(client => client.GetClient(config))
            .Returns(_client.Object);

        _repository = new HomepageRepository(config, contentfulClientManager.Object, _homepageFactory.Object, _cache.Object);
    }

    [Fact]
    public async Task Get_ReturnsHomepage()
    {
        // Arrange
        ContentfulCollection<ContentfulHomepage> collection = new()
        {
            Items = new List<ContentfulHomepage> { new ContentfulHomepage() }
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyWithLockingAsync(
                "test-homepage-tagId",
                It.IsAny<Func<Task<ContentfulHomepage>>>(),
                10,
                "Contentful"))
            .Returns((string cacheKey, Func<Task<ContentfulHomepage>> fallback, int minutes, string dataSource) => fallback());

        _homepageFactory
            .Setup(homepage => homepage.ToModel(It.IsAny<ContentfulHomepage>()))
            .Returns(new Homepage(string.Empty,
                                string.Empty,
                                new List<SubItem>(),
                                new List<SubItem>(),
                                new List<Alert>(),
                                new List<CarouselContent>(),
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                new CarouselContent(),
                                new CallToActionBanner(),
                                new CallToActionBanner(),
                                null));

        // Act
        HttpResponse response = await _repository.Get("tagId");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _cache.Verify(cache => cache.GetFromCacheOrDirectlyWithLockingAsync(
            "test-homepage-tagId",
            It.IsAny<Func<Task<ContentfulHomepage>>>(),
            10,
            "Contentful"), Times.Once);
        _client.Verify(client => client.GetEntries(
            It.IsAny<QueryBuilder<ContentfulHomepage>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}