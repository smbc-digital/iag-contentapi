namespace StockportContentApiTests.Unit.Repositories;

public class SiteHeaderRepositoryTests
{
    private readonly ContentfulConfig _config;
    private readonly Mock<IContentfulClient> _client;
    private readonly SiteHeaderRepository _repository;
    private readonly Mock<IContentfulFactory<ContentfulSiteHeader, SiteHeader>> _contentfulFactory;

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

        _client = new Mock<IContentfulClient>();
        _contentfulFactory = new Mock<IContentfulFactory<ContentfulSiteHeader, SiteHeader>>();

        contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);

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
            .Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulSiteHeader>>(
                    q => q.Build().Equals(new QueryBuilder<ContentfulSiteHeader>().ContentTypeIs("header").Include(1).Build())),
                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(siteHeaderCollection);

        _contentfulFactory
            .Setup(o => o.ToModel(It.IsAny<ContentfulSiteHeader>()))
            .Returns(new SiteHeader("Title", new List<SubItem>(), "Logo"));

        // Act
        HttpResponse siteHeader = AsyncTestHelper.Resolve(_repository.GetSiteHeader());

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
            .Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulSiteHeader>>(
                    q => q.Build().Equals(new QueryBuilder<ContentfulSiteHeader>().ContentTypeIs("header").Include(1).Build())),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(siteHeaderCollection);

        _contentfulFactory
            .Setup(o => o.ToModel(It.IsAny<ContentfulSiteHeader>()))
            .Returns((SiteHeader)null);

        // Act
        HttpResponse siteHeader = AsyncTestHelper.Resolve(_repository.GetSiteHeader());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, siteHeader.StatusCode);
    }
}
