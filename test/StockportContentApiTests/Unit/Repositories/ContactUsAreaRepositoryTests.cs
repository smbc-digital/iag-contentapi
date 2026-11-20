namespace StockportContentApiTests.Unit.Repositories;
public class ContactUsAreaRepositoryTests
{    
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContactUsAreaContentfulFactory> _contentfulFactory = new();
    private readonly Mock<ICache> _cache = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly ContactUsAreaRepository _repository;
    private readonly ContactUsArea _contactUsArea = new("title",
                                                        "slug",
                                                        new List<Crumb>(),
                                                        new List<Alert>(),
                                                        new List<SubItem> { new() { Title = "ContentBlock 1" }, new() { Title = "ContentBlock 2" } },
                                                        new List<ContactUsCategory> { new("title", "bodyTextLeft", "bodyTextRight", "icon") },
                                                        "title",
                                                        "body",
                                                        "meta description");

    public ContactUsAreaRepositoryTests()
    {
        ContentfulConfig config = BuildContentfulConfig();
        Mock<IContentfulClientManager> contentfulClientManager = SetupContentfulClientManager(config);
        ContentfulContactUsArea contentfulContactUsArea = new ContentfulContactUsAreaBuilder().Build();
        ContentfulCollection<ContentfulContactUsArea> contentfulCollection = new() { Items = [contentfulContactUsArea] };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulContactUsArea>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);
        
        _contentfulFactory
            .Setup(factory => factory.ToCacheModel(contentfulContactUsArea))
            .Returns(_contactUsArea);

        _contentfulFactory
            .Setup(factory => factory.FilterForCurrentDate(It.IsAny<ContactUsArea>()))
            .Returns<ContactUsArea>(contactUsArea => contactUsArea);

        _configuration
            .Setup(configuration => configuration["redisExpiryTimes:ContactUsArea"])
            .Returns("60");

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyWithLockingAsync(
                "test-contact-us-area-mapped",
                It.IsAny<Func<Task<ContactUsArea>>>(),
                60))
            .Returns((string _, Func<Task<ContactUsArea>> fallback, int _) => fallback());

        _repository = new ContactUsAreaRepository(
            config,
            contentfulClientManager.Object,
            _contentfulFactory.Object,
            _cache.Object,
            _configuration.Object);
    }

    private Mock<IContentfulClientManager> SetupContentfulClientManager(ContentfulConfig config)
    {
        Mock<IContentfulClientManager> contentfulClientManager = new();
        
        contentfulClientManager
            .Setup(client => client.GetClient(config))
            .Returns(_contentfulClient.Object);
    
        return contentfulClientManager;
    }

    private static ContentfulConfig BuildContentfulConfig() =>
        new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

    [Fact]
    public async Task GetContactUsArea_ReturnsNotFound_WhenContactUsAreaIsNull()
    {
        // Arrange
        ContentfulCollection<ContentfulContactUsArea> _contactUsArea = new() { Items = [] };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulContactUsArea>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_contactUsArea);

        // Act
        HttpResponse response = await _repository.GetContactUsArea("tagId");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No contact us area found", response.Error);
    }

    [Fact]
    public async Task GetContactUsArea_ReturnsSuccess_WhenContactUsAreaIsNotNull()
    {
        // Arrange
        _contentfulFactory
            .Setup(factory => factory.ToCacheModel(It.IsAny<ContentfulContactUsArea>()))
            .Returns(_contactUsArea);

        // Act
        HttpResponse response = await _repository.GetContactUsArea("tagId");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _cache.Verify(cache => cache.GetFromCacheOrDirectlyWithLockingAsync(
            "test-contact-us-area-mapped",
            It.IsAny<Func<Task<ContactUsArea>>>(),
            60), Times.Once);
        _contentfulFactory.Verify(factory => factory.ToCacheModel(It.IsAny<ContentfulContactUsArea>()), Times.Once);
    }
}