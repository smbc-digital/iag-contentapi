namespace StockportContentApiTests.Unit.Repositories;

public class SiteHeaderRepositoryTests
{
    private readonly ContentfulConfig _config;
    private readonly Mock<IContentfulClient> _client = new();
    private readonly SiteHeaderRepository _repository;
    private readonly Mock<IContentfulFactory<ContentfulSiteHeader, SiteHeader>> _contentfulFactory = new();

    public SiteHeaderRepositoryTests()
    {
        _config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        Mock<IContentfulClientManager> contentfulClientManager = new();

        contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(_config))
            .Returns(_client.Object);

        _repository = new SiteHeaderRepository(_config, contentfulClientManager.Object, _contentfulFactory.Object);
    }

    [Fact]
    public void GetSiteHeader_ShouldReturnASiteHeader()
    {
        // Arrange
        SiteHeader mockSiteHeader = new("Title", new List<SubItem>(), "Logo");

        ContentfulCollection<ContentfulSiteHeader> siteHeaderCollection = new()
        {
            Items = new List<ContentfulSiteHeader>
            {
               new ContentfulSiteHeaderBuilder().Build()
            }
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulSiteHeader>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(siteHeaderCollection);

        _contentfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(It.IsAny<ContentfulSiteHeader>()))
            .Returns(new SiteHeader("Title", new List<SubItem>(), "Logo"));

        // Act
        HttpResponse siteHeader = AsyncTestHelper.Resolve(_repository.GetSiteHeader("tagId"));

        // Assert
        Assert.Equal(mockSiteHeader.Title, siteHeader.Get<SiteHeader>().Title);
        Assert.Equal(mockSiteHeader.Logo, siteHeader.Get<SiteHeader>().Logo);
        Assert.Equal(HttpStatusCode.OK, siteHeader.StatusCode);
    }

    [Fact]
    public void GetSiteHeader_ShouldReturnNotFound_IfSiteHeaderIsNull()
    {
        // Arrange
        SiteHeader mockSiteHeader = new("Title", new List<SubItem>(), "Logo");

        ContentfulCollection<ContentfulSiteHeader> siteHeaderCollection = new()
        {
            Items = new List<ContentfulSiteHeader>
            {
                new ContentfulSiteHeaderBuilder().Build()
            }
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulSiteHeader>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(siteHeaderCollection);

        _contentfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(It.IsAny<ContentfulSiteHeader>()))
            .Returns((SiteHeader)null);

        // Act
        HttpResponse siteHeader = AsyncTestHelper.Resolve(_repository.GetSiteHeader("tagId"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, siteHeader.StatusCode);
    }
}