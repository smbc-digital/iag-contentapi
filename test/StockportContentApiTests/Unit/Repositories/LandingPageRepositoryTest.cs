namespace StockportContentApiTests.Unit.Repositories;

public class LandingPageRepositoryTest
{
    private readonly LandingPageRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulLandingPage, LandingPage>> _contentfulFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulNews, News>> _newsFactory = new();

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
        _repository = new LandingPageRepository(config, _contentfulFactory.Object, contentfulClientManager.Object, _newsFactory.Object);
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
    public async Task GetLandingPage_PopulatesContentBlocks_WhenPageSectionsItemsArePresent()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new()
        {
            PageSections = new()
            {
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build()
            },
        };

        ContentBlock contentBlock1 = new() { Title = "ContentBlock 1" };
        ContentBlock contentBlock2 = new() { Title = "ContentBlock 2" };

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
            Icon = "icon",
            HeaderType = "full image",
            HeaderImage = new MediaAsset(),
            PageSections = new List<ContentBlock>() { contentBlock1, contentBlock2 }
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
        Assert.NotNull(responseLandingPage.PageSections);
        Assert.Equal(2, responseLandingPage.PageSections.Count());
        Assert.Equal(contentBlock1, responseLandingPage.PageSections.ToList()[0]);
        Assert.Equal(contentBlock2, responseLandingPage.PageSections.ToList()[1]);
    }

    [Fact]
    public async Task GetLandingPage_ShouldCallGetLatestNewsByCategory_WhenNewsBannerExists()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new()
        {
            PageSections = new()
            {
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build(),
                new ContentfulReferenceBuilder().Build()
            },
        };

        LandingPage landingPage = new()
        {
            Slug = "existing-slug",
            PageSections = new List<ContentBlock>
            {
                new()
                {
                    ContentType = "NewsBanner",
                    AssociatedTagCategory = "news-category"
                }
            }
        };

        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };

        ContentfulNews contentfulNews = new();
        News news = new(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<Crumb>>(),
            It.IsAny<List<Alert>>(), It.IsAny<List<string>>(), It.IsAny<List<Document>>(), It.IsAny<List<string>>(), It.IsAny<List<Profile>>());

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

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = new List<ContentfulNews> { new() } });

        _newsFactory.Setup(factory => factory.ToModel(new ContentfulNews()))
            .Returns(news);

        // Act
        HttpResponse response = await _repository.GetLandingPage("existing-slug");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestNewsByTagOrCategory_ShouldReturnNews_WhenCategoryMatches()
    {
        // Arrange
        ContentfulNews contentfulNews = new();
        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulNews>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = new List<ContentfulNews> { contentfulNews } });

        // Act
        ContentfulNews news = await _repository.GetLatestNewsByTagOrCategory("some-category");

        // Assert
        Assert.NotNull(news);
        Assert.Equal(contentfulNews, news);
        _contentfulClient.Verify(client => client.GetEntries(It.Is<QueryBuilder<ContentfulNews>>(q => q.Build().Contains("tags")), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetLatestNewsByTagOrCategory_ShouldReturnNews_WhenTagMatchesAfterCategoryFails()
    {
        // Arrange
        ContentfulNews contentfulNews = new();
        
        _contentfulClient.Setup(client => client.GetEntries(It.Is<QueryBuilder<ContentfulNews>>(q => q.Build().Contains("categories")), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContentfulCollection<ContentfulNews>)null);

        _contentfulClient.Setup(client => client.GetEntries(It.Is<QueryBuilder<ContentfulNews>>(q => q.Build().Contains("tags")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulNews> { Items = new List<ContentfulNews> { contentfulNews } });

        // Act
        ContentfulNews news = await _repository.GetLatestNewsByTagOrCategory("some-tag");

        // Assert
        Assert.NotNull(news);
        Assert.Equal(contentfulNews, news);
        _contentfulClient.Verify(client => client.GetEntries(It.Is<QueryBuilder<ContentfulNews>>(q => q.Build().Contains("categories")), It.IsAny<CancellationToken>()), Times.Once);
        _contentfulClient.Verify(client => client.GetEntries(It.Is<QueryBuilder<ContentfulNews>>(q => q.Build().Contains("tags")), It.IsAny<CancellationToken>()), Times.Once);
    }
}