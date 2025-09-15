using Microsoft.Extensions.Options;
using StockportContentApiTests.Unit.ContentfulFactories;
using Xunit.Sdk;

namespace StockportContentApiTests.Unit.Repositories;

public class ArticleRepositoryTests
{
    private readonly Mock<ICache> _cacheWrapper = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly ArticleRepository _repository;
    private readonly Mock<ITimeProvider> _mockTimeProvider = new();
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IVideoRepository> _videoRepository = new();
    private readonly Mock<EventRepository> _eventRepository = new();
    private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _sectionFactory = new();
    private readonly Mock<IOptions<RedisExpiryConfiguration>> _mockOptions = new();
    private readonly Mock<ICache> _cache = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory = new();
    
    public ArticleRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();
        
        CacheKeyConfig cacheKeyconfig = new CacheKeyConfig("test")
            .Add("TEST_EventsCacheKey", "testEventsCacheKey")
            .Add("TEST_NewsCacheKey", "testNewsCacheKey")
            .Build();

        _videoRepository
            .Setup(videoRepository => videoRepository.Process(It.IsAny<string>()))
            .Returns(string.Empty);

        _mockOptions
            .Setup(options => options.Value)
            .Returns(new RedisExpiryConfiguration { Articles = 60 });

        ArticleContentfulFactory contentfulFactory = new(
            _sectionFactory.Object,
            new Mock<IContentfulFactory<ContentfulReference, Crumb>>().Object,
            new Mock<IContentfulFactory<ContentfulProfile, Profile>>().Object,
            new Mock<IContentfulFactory<ContentfulArticle, Topic>>().Object,
            new DocumentContentfulFactory(),
            _videoRepository.Object,
            _mockTimeProvider.Object,
            new Mock<IContentfulFactory<ContentfulAlert, Alert>>().Object,
            new Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>>().Object,
            new Mock<IContentfulFactory<ContentfulReference, SubItem>>().Object,
            new Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>>().Object,
            new Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>().Object
        );

        _mockTimeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 10, 15));

        ContentfulCollection<ContentfulArticle> collection = new()
        {
            Items = new List<ContentfulArticle>()
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(config))
            .Returns(_contentfulClient.Object);

        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(DateTime.Today.AddDays(1));

        _eventRepository = new(config,
                            cacheKeyconfig,
                            contentfulClientManager.Object,
                            _timeProvider.Object,
                            _eventFactory.Object,
                            _eventHomepageFactory.Object,
                            _cacheWrapper.Object,
                            _configuration.Object);

        _repository = new(config,
                        contentfulClientManager.Object,
                        _mockTimeProvider.Object,
                        contentfulFactory,
                        new ArticleSiteMapContentfulFactory(),
                        _videoRepository.Object,
                        _eventRepository.Object,
                        _cache.Object,
                        _mockOptions.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnSuccessfulResponse()
    {
        // Arrange
        ContentfulCollection<ContentfulArticleForSiteMap> collection = new()
        {
            Items = new List<ContentfulArticleForSiteMap>()
            {
                new ContentfulArticleForSiteMapBuilder().Build()
            }
        };
        
        string builder = new QueryBuilder<ContentfulArticleForSiteMap>()
            .ContentTypeIs("article")
            .Include(2)
            .Build();

        _contentfulClient
            .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulArticleForSiteMap>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.Get();
        IEnumerable<ArticleSiteMap> responseArticle = response.Get<IEnumerable<ArticleSiteMap>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(collection.Items.Count(), responseArticle.ToList().Count);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFoundResponse_IfArticleDoesNotExist()
    {
        // Act
        HttpResponse response = await _repository.Get();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFoundResponse_IfArticleNotWithinDates()
    {
        // Arrange
        ContentfulCollection<ContentfulArticleForSiteMap> collection = new()
        {
            Items = new List<ContentfulArticleForSiteMap>()
            {
                new ContentfulArticleForSiteMapBuilder()
                    .WithSunrise(DateTime.Now.AddDays(-1))
                    .WithSunset(DateTime.Now.AddDays(2))
                    .Build()
            }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulArticleForSiteMap>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.Get();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(response.Error);
        Assert.Contains("No articles found within sunrise and sunset dates", response.Error);
    }

    [Fact]
    public void GetArticle_ShouldReturnSuccessfulResponse()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("article-unit-test-article")), It.IsAny<Func<Task<ContentfulArticle>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new ContentfulArticleBuilder().Slug("unit-test-article").WithAssociatedTagCategory(string.Empty).Build());

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        // Assert
        Article resultArticle = response.Get<Article>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(resultArticle.Events);

        _eventRepository.Verify(repo => repo.GetEventsByCategory(It.IsAny<string>(), true), Times.Never);
        _eventRepository.Verify(repo => repo.GetEventsByTag(It.IsAny<string>(), true), Times.Never);
    }

    [Fact]
    public void GetArticle_ShouldReturnNotFoundResponse_IfArticleDoesNotExist()
    {
        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("slug"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No article found for 'slug'", response.Error);
    }

    [Fact]
    public void GetArticle_ShouldReturnNotFoundResponse_ForNewsOutsideOfSunriseDate()
    {
        // Arrange
        _mockTimeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 01, 01));

        ContentfulCollection<ContentfulArticle> collection = new()
        {
            Items = new List<ContentfulArticle>
            {
                new ContentfulArticleBuilder().Slug("unit-test-article").Build()
            }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public void GetArticle_ShouldReturnNotFoundResponse_ForNewsOutsideOfSunsetDate()
    {
        // Arrange
        _mockTimeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2017, 08, 01));

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public void GetArticle_ShouldReturnValidSunsetAndSunriseDateWhenDateInRange()
    {
        // Arrange
        _mockTimeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 08, 01));

        ContentfulArticle rawArticle = new ContentfulArticleBuilder()
            .Slug("unit-test-article")
            .WithAssociatedTagCategory(string.Empty)
            .Build();

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("article-unit-test-article")), It.IsAny<Func<Task<ContentfulArticle>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(rawArticle);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void GetArticle_ShouldReturnArticleWithInlineAlerts()
    {
        // Arrange
        List<ContentfulAlert> alertsInline = new(){
            new ContentfulAlertBuilder().Build()
        };

        ContentfulArticle rawArticle = new ContentfulArticleBuilder().Slug("unit-test-article-with-inline-alerts").WithAssociatedTagCategory(string.Empty).AlertsInline(alertsInline).Build();
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("article-unit-test-article-with-inline-alerts")), It.IsAny<Func<Task<ContentfulArticle>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(rawArticle);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article-with-inline-alerts"));

        // Assert
        Article article = response.Get<Article>();
        Assert.NotNull(article.AlertsInline);
    }

    [Fact]
    public void GetArticle_ShouldReturnArticleWithASectionThatHasAnInlineAlert()
    {
        // Arrange
        Alert alert = new AlertBuilder().Build();

        ContentfulCollection<ContentfulArticle> collection = new();
        ContentfulArticle rawArticle = new ContentfulArticleBuilder()
            .Slug("unit-test-article-with-section-with-inline-alerts")
            .WithAssociatedTagCategory(string.Empty)
            .Build();

        collection.Items = new List<ContentfulArticle> { rawArticle };
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("article-unit-test-article-with-section-with-inline-alerts")), It.IsAny<Func<Task<ContentfulArticle>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(rawArticle);

        _sectionFactory
            .Setup(sectionFactory => sectionFactory.ToModel(It.IsAny<ContentfulSection>()))
            .Returns(new Section()
                {
                    AlertsInline = new List<Alert>() { alert }
                });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article-with-section-with-inline-alerts"));

        // Assert
        Article article = response.Get<Article>();
        Assert.Equal(alert, article.Sections[0].AlertsInline.First());
    }

    [Fact]
    public void GetArticle_WithMultipleAssociatedTagCategories_ShouldFetchDistinctTop3Events()
    {
        // Arrange
        List<Event> eventsFromCategory = new()
        {
            new EventBuilder().Slug("event2").Build(),
            new EventBuilder().Slug("event3").Build(),
            new EventBuilder().Slug("event4").Build()
        };

        List<Event> eventsFromTag = new()
        {
            new EventBuilder().Slug("event4").Build(),
            new EventBuilder().Slug("event5").Build(),
            new EventBuilder().Slug("event6").Build()
        };

        ContentfulArticle article = new ContentfulArticleBuilder().Slug("unit-test-article").WithAssociatedTagCategory("dance, tag1").Build();
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("article-unit-test-article")), It.IsAny<Func<Task<ContentfulArticle>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(article);

        _eventRepository
            .Setup(repo => repo.GetEventsByCategory("dance", true))
            .ReturnsAsync(eventsFromCategory);

        _eventRepository
            .Setup(repo => repo.GetEventsByTag("tag1", true))
            .ReturnsAsync(eventsFromTag);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        // Assert
        Article resultArticle = response.Get<Article>();
        Assert.NotNull(resultArticle);
        Assert.Equal(3, resultArticle.Events.Count);
        Assert.Contains(resultArticle.Events, e => e.Slug.Equals("event2"));
        Assert.Contains(resultArticle.Events, e => e.Slug.Equals("event3"));
        Assert.Contains(resultArticle.Events, e => e.Slug.Equals("event4"));

        _eventRepository.Verify(repo => repo.GetEventsByCategory("dance", true), Times.Once);
    }
}