namespace StockportContentApiTests.Unit.Repositories;

public class PrivacyNoticeRepositoryTest
{
    private readonly PrivacyNoticeRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>> _contentfulFactory;


    public PrivacyNoticeRepositoryTest()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _contentfulFactory = new Mock<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>>();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();

        contentfulClientManager
            .Setup(o => o.GetClient(config))
            .Returns(_contentfulClient.Object);

        _repository = new PrivacyNoticeRepository(config, _contentfulFactory.Object, contentfulClientManager.Object);
    }

    [Fact]
    public void GetPrivacyNotice_ShouldCallContentful()
    {
        // Arrange
        const string slug = "test-slug";
        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Slug = slug
        };

        PrivacyNotice privacyNotice = new()
        {
            Slug = slug
        };

        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
        _contentfulFactory.Setup(_ => _.ToModel(contentfulPrivacyNotice)).Returns(privacyNotice);

        // Act
        PrivacyNotice result = AsyncTestHelper.Resolve(_repository.GetPrivacyNotice(slug));
        
        // Assert
        _contentfulClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void GetPrivacyNotice_ShouldReturnAPrivacyNotice()
    {
        // Arrange
        const string slug = "test-slug";
        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Slug = slug
        };

        PrivacyNotice privacyNotice = new()
        {
            Slug = slug
        };

        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
        _contentfulFactory.Setup(_ => _.ToModel(contentfulPrivacyNotice)).Returns(privacyNotice);

        // Act
        var result = AsyncTestHelper.Resolve(_repository.GetPrivacyNotice(slug));

        // Assert
        Assert.IsType<PrivacyNotice>(result);
        _contentfulFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulPrivacyNotice>()), Times.Once);
    }

     [Fact]
    public void GetPrivacyNotice_ShouldReturnNull()
    {
        // Arrange
        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { null }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        // Act
        var result = AsyncTestHelper.Resolve(_repository.GetPrivacyNotice("slug-that-returns-nothing"));

        // Assert
        Assert.Null(result);
        _contentfulFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulPrivacyNotice>()), Times.Never);
    }

    [Fact]
    public void GetAllPrivacyNotices_ShouldCallContentful()
    {
        // Arrange
        const string slug = "test-slug";
        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Slug = slug
        };

        PrivacyNotice privacyNotice = new()
        {
            Slug = slug
        };

        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
        _contentfulFactory.Setup(_ => _.ToModel(contentfulPrivacyNotice)).Returns(privacyNotice);

        // Act
        List<PrivacyNotice> result = AsyncTestHelper.Resolve(_repository.GetAllPrivacyNotices());
        
        // Assert
        _contentfulClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void GetPrivacyNoticesByTitle_ShouldCallContentful()
    {
        // Arrange
        const string slug = "test-slug";
        const string title = "test title";
        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Slug = slug,
            Title = title
        };

        PrivacyNotice privacyNotice = new()
        {
            Slug = slug,
            Title = title
        };

        ContentfulCollection<ContentfulPrivacyNotice> contentfulCollection = new()
        {
            Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
        _contentfulFactory.Setup(_ => _.ToModel(contentfulPrivacyNotice)).Returns(privacyNotice);

        // Act
        List<PrivacyNotice> result = AsyncTestHelper.Resolve(_repository.GetPrivacyNoticesByTitle(title));
        
        // Assert
        _contentfulClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}