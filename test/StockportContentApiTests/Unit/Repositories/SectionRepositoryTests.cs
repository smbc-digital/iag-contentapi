namespace StockportContentApiTests.Unit.Repositories;

public class SectionRepositoryTests
{
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _contentfulFactory = new();
    private readonly SectionRepository _repository;
    private readonly Section _section = new();

    public SectionRepositoryTests()
    {
        ContentfulConfig config = BuildContentfulConfig();
        
        Mock<IContentfulClientManager> contentfulClientManager = SetupContentfulClientManager(config);
        
        _repository = new(config, _contentfulFactory.Object, contentfulClientManager.Object);

        ContentfulSection contentfulSection = new ContentfulSectionBuilder().Build();

        ContentfulCollection<ContentfulSection> contentfulCollection = new() { Items = [contentfulSection] };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulSection>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        _contentfulFactory
            .Setup(factory => factory.ToModel(contentfulSection))
            .Returns(_section);
    }

    private static ContentfulConfig BuildContentfulConfig() =>
        new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

    private Mock<IContentfulClientManager> SetupContentfulClientManager(ContentfulConfig config)
    {
        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(config))
            .Returns(_contentfulClient.Object);
        
        return contentfulClientManager;
    }

    [Fact]
    public async Task GetSections_ReturnsSuccessResponse_WhenSectionIsFound()
    {
        // Act
        HttpResponse response = await _repository.GetSections("section-slug");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsSuccessResponse_WhenSectionsFound()
    {
        // Arrange
        ContentfulCollection<ContentfulArticleForSiteMap> articleCollection = new()
        {
            Items = new List<ContentfulArticleForSiteMap> ()
            {
                new ContentfulArticleForSiteMapBuilder().Build()
            }
        };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulArticleForSiteMap>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(articleCollection);

        // Act
        HttpResponse response = await _repository.Get();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}