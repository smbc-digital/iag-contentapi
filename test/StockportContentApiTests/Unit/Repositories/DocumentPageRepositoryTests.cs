namespace StockportContentApiTests.Unit.Repositories;

public class DocumentPageRepositoryTests
{
    private readonly DocumentPageRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<ICache> _cache = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();

    public DocumentPageRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 10, 15));

        DocumentPageContentfulFactory contentfulFactory = new(new Mock<IContentfulFactory<Asset, Document>>().Object,
                                                            new Mock<IContentfulFactory<ContentfulReference, SubItem>>().Object,
                                                            new Mock<IContentfulFactory<ContentfulReference, Crumb>>().Object,
                                                            _timeProvider.Object);

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(config))
            .Returns(_contentfulClient.Object);
        
        _repository = new(config,
                        contentfulClientManager.Object,
                        contentfulFactory,
                        _cache.Object);
    }

    [Fact]
    public async Task GetDocumentPage_ShouldReturnNotFound_If_DocumentDoesNotExist()
    {
        // Act
        HttpResponse response = await _repository.GetDocumentPage("slug", "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDocumentPage_ShouldReturnSuccessfulResponse()
    {
        // Arrange
        ContentfulDocumentPage contentfulDocumentPage = new ContentfulDocumentPageBuilder().Build();

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(slug => slug.Equals($"documentPage-slug")), It.IsAny<Func<Task<ContentfulDocumentPage>>>()))
            .ReturnsAsync(contentfulDocumentPage);

        // Act
        HttpResponse response = await _repository.GetDocumentPage("slug", "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDocumentPageEntry_ShouldReturnDocumentPage_WhenEntryExistsAsync()
    {
        // Arrange
        ContentfulCollection<ContentfulDocumentPage> contentfulCollection = new()
        {
            Items = new List<ContentfulDocumentPage> { new ContentfulDocumentPageBuilder().Build() }
        };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulDocumentPage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        // Act
        ContentfulDocumentPage result = await _repository.GetDocumentPageEntry("slug", "tagId");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("slug", result.Slug);
        _contentfulClient.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulDocumentPage>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDocumentPageEntry_ShouldReturnNull_WhenNoEntryExists()
    {
        // Arrange
        ContentfulCollection<ContentfulDocumentPage> contentfulCollection = new()
        {
            Items = new List<ContentfulDocumentPage>()
        };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulDocumentPage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        // Act
        ContentfulDocumentPage result = await _repository.GetDocumentPageEntry("non-existing-slug", "tagId");

        // Assert
        Assert.Null(result);
        _contentfulClient.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulDocumentPage>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}