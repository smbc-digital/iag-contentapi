namespace StockportContentApiTests.Unit.Repositories;

public class LandingPageRepositoryTest
{
    private readonly LandingPageRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulLandingPage, LandingPage>> _contentfulFactory = new();
    private readonly Mock<ICache> _cache;

    public LandingPageRepositoryTest()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(_ => _.GetClient(config)).Returns(_contentfulClient.Object);
        _repository = new LandingPageRepository(config, _contentfulFactory.Object, contentfulClientManager.Object);
    }

    [Fact]
    public async Task GetLandingPage_ReturnsSuccessResponse_WhenLandingPageIsFound()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new() { };
        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        _contentfulFactory.Setup(factory => factory.ToModel(contentfulLandingPage))
            .Returns(new LandingPage());

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLandingPage_ReturnsNotFoundResponse_WhenLandingPageIsNotFound()
    {
        // Arrange
        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = Enumerable.Empty<ContentfulLandingPage>() };
        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        // Act
        HttpResponse response = await _repository.GetLandingPage("non-existent-slug");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No Landing Page found", response.Error);
    }

    [Fact]
    public async Task GetLandingPage_PopulatesContentBlocks_WhenContentReferenceItemsArePresent()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new()
        {
            ContentReference = new()
            {
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build()
            },
        };

        SubItem subItem1 = new() { Title = "SubItem 1" };
        SubItem subItem2 = new() { Title = "SubItem 2" };

        LandingPage landingPage = new()
        {
            Slug = "landing-page-slug",
            Title = "landing page title",
            Subtitle = "landing page subtitle",
            Breadcrumbs = new List<Crumb>(),
            Alerts = new List<Alert>(),
            Teaser = "landing page teaser",
            MetaDescription = "landing page metadescription",
            Image = new MediaAsset(),
            HeaderType = "full image",
            HeaderImage = new MediaAsset(),
            ContentReference = new List<SubItem>() { subItem1, subItem2 }
        };

        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        _contentfulFactory.Setup(factory => factory.ToModel(contentfulLandingPage))
            .Returns(landingPage);

        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulReference>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulReference>
            {
                Items = new List<ContentfulReference>
                {
                    new() { Sys = new SystemProperties { Id = "content-block-1" } },
                    new() { Sys = new SystemProperties { Id = "content-block-2" } }
                }
            });

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug");
        LandingPage responseLandingPage = response.Get<LandingPage>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseLandingPage.ContentReference);
        Assert.Equal(2, responseLandingPage.ContentReference.Count());
        Assert.Equal(subItem1, responseLandingPage.ContentReference.ToList()[0]);
        Assert.Equal(subItem2, responseLandingPage.ContentReference.ToList()[1]);
    }
}