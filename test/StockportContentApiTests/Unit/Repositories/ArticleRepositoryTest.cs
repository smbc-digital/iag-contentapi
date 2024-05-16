using Microsoft.Extensions.Options;

namespace StockportContentApiTests.Unit.Repositories;

public class ArticleRepositoryTest
{
    private readonly ArticleRepository _repository;
    private readonly Mock<ITimeProvider> _mockTimeProvider;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IVideoRepository> _videoRepository;
    private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _sectionFactory;
    private readonly Mock<IOptions<RedisExpiryConfiguration>> _mockOptions;
    private readonly Mock<ICache> _cache;

    public ArticleRepositoryTest()
    {
        var config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Build();
        
        _videoRepository = new Mock<IVideoRepository>();
        _videoRepository.Setup(_ => _.Process(It.IsAny<string>())).Returns(string.Empty);
        _mockTimeProvider = new Mock<ITimeProvider>();
        _sectionFactory = new Mock<IContentfulFactory<ContentfulSection, Section>>();

        _cache = new Mock<ICache>();
        _mockOptions = new Mock<IOptions<RedisExpiryConfiguration>>();
        _mockOptions.Setup(options => options.Value).Returns(new RedisExpiryConfiguration {  Articles = 60 });

        var contentfulFactory = new ArticleContentfulFactory(
            _sectionFactory.Object,
            new Mock<IContentfulFactory<ContentfulReference, Crumb>>().Object,
            new Mock<IContentfulFactory<ContentfulProfile, Profile>>().Object,
            new Mock<IContentfulFactory<ContentfulArticle, Topic>>().Object,
            new DocumentContentfulFactory(),
            _videoRepository.Object,
            _mockTimeProvider.Object,
            new Mock<IContentfulFactory<ContentfulAlert, Alert>>().Object
        );

        var contentfulClientManager = new Mock<IContentfulClientManager>();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(_ => _.GetClient(config)).Returns(_contentfulClient.Object);
        _repository = new ArticleRepository(config, contentfulClientManager.Object, _mockTimeProvider.Object, contentfulFactory, new ArticleSiteMapContentfulFactory(), _videoRepository.Object, _cache.Object, _mockOptions.Object);
    }

    [Fact]
    public async void Get_ShouldReturnSuccessfulResponse()
    {
        // Arrange
        ContentfulCollection<ContentfulArticleForSiteMap> collection = new()
        {
            Items = new List<ContentfulArticleForSiteMap>()
            {
                new()
                {
                    Slug = "slug1",
                    SunriseDate = DateTime.MinValue,
                    SunsetDate = DateTime.MaxValue
                },
                new()
                {
                    Slug = "slug2",
                    SunriseDate = DateTime.MinValue,
                    SunsetDate = DateTime.MaxValue
                },
                new()
                {
                    Slug = "slug3",
                    SunriseDate = DateTime.MinValue,
                    SunsetDate = DateTime.MaxValue
                }
            }
        };

        var builder = new QueryBuilder<ContentfulArticleForSiteMap>()
            .ContentTypeIs("article")
            .Include(2)
            .Build();

        _contentfulClient
            .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulArticleForSiteMap>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        var response = await _repository.Get();
        var responseArticle = response.Get<IEnumerable<ArticleSiteMap>>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(collection.Items.Count(), responseArticle.ToList().Count);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFoundResponse_IfArticleDoesNotExist(){
        // Arrange
        ContentfulCollection<ContentfulArticle> collection = new()
        {
            Items = new List<ContentfulArticle>()
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        // Act
        var response = await _repository.Get();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFoundResponse_IfArticleNotWithinDates(){
        // Arrange
        ContentfulCollection<ContentfulArticleForSiteMap> collection = new()
        {
            Items = new List<ContentfulArticleForSiteMap>()
            {
                new()
                {
                    Slug = "slug1",
                    SunriseDate = DateTime.Now.AddDays(-1),
                    SunsetDate = DateTime.Now.AddDays(2)
                },
                new()
                {
                    Slug = "slug2",
                    SunriseDate = DateTime.Now.AddDays(2),
                    SunsetDate = DateTime.Now.AddDays(5)
                },
                new()
                {
                    Slug = "slug3",
                    SunriseDate = DateTime.Now.AddDays(-2),
                    SunsetDate = DateTime.Now.AddDays(3)
                }
            }
        };

        _contentfulClient
            .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulArticleForSiteMap>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        var response = await _repository.Get();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(response.Error);
        Assert.Contains("No articles found within sunrise and sunset dates", response.Error);
    }

    [Fact]
    public void GetArticle_ShouldReturnSuccessfulResponse()
    {
        // Arrange
        const string slug = "unit-test-article";
        var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 10, 15));


        _cache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"article-{slug}")), It.IsAny<Func<Task<ContentfulArticle>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(rawArticle);
        
        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetArticle(slug));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void GetArticle_ShouldReturnNotFoundResponse_IfArticleDoesNotExist()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 10, 15));

        ContentfulCollection<ContentfulArticle> collection = new()
        {
            Items = new List<ContentfulArticle>()
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);
        
        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetArticle("slug"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Error.Should().Be("No article found for 'slug'");
    }

    [Fact]
    public void GetArticle_ShouldReturnNotFoundResponse_ForNewsOutsideOfSunriseDate()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 01, 01));

        var rawArticle = new ContentfulArticleBuilder().Slug("unit-test-article").Build();

        ContentfulCollection<ContentfulArticle> collection = new()
        {
            Items = new List<ContentfulArticle> { rawArticle }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void GetArticle_ShouldRetunNotFoundResponse_ForNewsOutsideOfSunsetDate()
    {
        // Arrange
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2017, 08, 01));
        var rawArticle = new ContentfulArticleBuilder().Slug("unit-test-article").Build();

        ContentfulCollection<ContentfulArticle> collection = new()
        {
            Items = new List<ContentfulArticle> { rawArticle }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void GetArticle_ShouldReturnValidSunsetAndSunriseDateWhenDateInRange()
    {
        // Arrange
        const string slug = "unit-test-article";
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 01));

        var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();

        _cache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"article-{slug}")), It.IsAny<Func<Task<ContentfulArticle>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(rawArticle);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void GetArticle_ShouldReturnArticleWithInlineAlerts()
    {
        // Arrange
        const string slug = "unit-test-article-with-inline-alerts";
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 10, 15));
        List<ContentfulAlert> alertsInline = new(){ 
            new()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(2017, 05, 01),
                SunsetDate = new DateTime(2017, 07, 01),
                Sys = new SystemProperties() { Type = "Entry" }
            }
        };

        var rawArticle = new ContentfulArticleBuilder().Slug(slug).AlertsInline(alertsInline).Build();
        _cache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"article-{slug}")), It.IsAny<Func<Task<ContentfulArticle>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(rawArticle);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetArticle(slug));

        // Assert
        var article = response.Get<Article>();
        article.AlertsInline.Should().NotBeNull();
    }

    [Fact]
    public void GetArticle_ShouldReturnArticleWithASectionThatHasAnInlineAlert()
    {
        // Arrange
        const string slug = "unit-test-article-with-section-with-inline-alerts";
        _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));
        var alert = new Alert("title", "subHeading", "body", "severity",
                    new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc), string.Empty, false, string.Empty);
        var collection = new ContentfulCollection<ContentfulArticle>();
        var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();
        collection.Items = new List<ContentfulArticle> { rawArticle };
        _cache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"article-{slug}")), It.IsAny<Func<Task<ContentfulArticle>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(rawArticle);
        _sectionFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulSection>())).Returns(new Section(
            "title",
            "section-with-inline-alerts",
            "metaDescription",
            "body",
            new List<Profile>(),
            new List<Document>(),
            new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc),
            new List<Alert> { alert }));

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetArticle(slug));

        // Assert
        var article = response.Get<Article>();
        article.Sections[0].AlertsInline.Should().Equal(alert);
    }
}