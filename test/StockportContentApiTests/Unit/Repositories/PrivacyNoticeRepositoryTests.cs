namespace StockportContentApiTests.Unit.Repositories;

public class PrivacyNoticeRepositoryTests
{
    private readonly PrivacyNoticeRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>> _contentfulFactory = new();
    private readonly ContentfulPrivacyNotice contentfulPrivacyNotice = new ContentfulPrivacyNoticeBuilder().Build();
    private readonly PrivacyNotice privacyNotice = new()
    {
        Slug = "test-slug",
        Title = "test title"
    };
    
    public PrivacyNoticeRepositoryTests()
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
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(config))
            .Returns(_contentfulClient.Object);

        _repository = new PrivacyNoticeRepository(config, _contentfulFactory.Object, contentfulClientManager.Object);
    }

    [Fact]
    public async Task GetPrivacyNotice_ShouldCallContentful()
    {
        // Arrange
        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        _contentfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(contentfulPrivacyNotice))
            .Returns(privacyNotice);

        // Act
        HttpResponse result = await _repository.GetPrivacyNotice("test-slug");

        // Assert
        _contentfulClient.Verify(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPrivacyNotice_ShouldReturnHttpResponseWithPrivacyNotice()
    {
        // Arrange
        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        _contentfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(contentfulPrivacyNotice))
            .Returns(privacyNotice);

        // Act
        HttpResponse result = await _repository.GetPrivacyNotice("test-slug");

        // Assert
        Assert.IsType<HttpResponse>(result);
        _contentfulFactory.Verify(contentfulFactory => contentfulFactory.ToModel(It.IsAny<ContentfulPrivacyNotice>()), Times.Once);
    }

    [Fact]
    public async Task GetPrivacyNotice_ShouldReturnNull()
    {
        // Arrange
        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { null }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        // Act
        HttpResponse result = await _repository.GetPrivacyNotice("slug-that-returns-nothing");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        _contentfulFactory.Verify(contentfulFactory => contentfulFactory.ToModel(It.IsAny<ContentfulPrivacyNotice>()), Times.Never);
    }

    [Fact]
    public async Task GetAllPrivacyNotices_ShouldCallContentful()
    {
        // Arrange
        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        _contentfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(contentfulPrivacyNotice))
            .Returns(privacyNotice);

        // Act
        HttpResponse result = await _repository.GetAllPrivacyNotices();

        // Assert
        _contentfulClient.Verify(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPrivacyNoticesByTitle_ShouldCallContentful()
    {
        // Arrange
        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        _contentfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(contentfulPrivacyNotice))
            .Returns(privacyNotice);

        // Act
        List<PrivacyNotice> result = await _repository.GetPrivacyNoticesByTitle("test title");

        // Assert
        _contentfulClient.Verify(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}