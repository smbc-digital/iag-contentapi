using System.Threading.Tasks;

namespace StockportContentApiTests.Unit.Repositories;

public class RedirectsRepositoryTests
{
    private readonly ContentfulConfig _config;
    private readonly Mock<Func<string, ContentfulConfig>> _createConfig = new();
    private readonly Mock<IContentfulClientManager> _contentfulClientManager = new();
    private readonly Mock<IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>> _contenfulFactory = new();
    private readonly Mock<IContentfulClient> _client = new();
    private readonly ShortUrlRedirects _shortUrlRedirects = new(new Dictionary<string, RedirectDictionary>());
    private readonly LegacyUrlRedirects _legacyUrlRedirects = new(new Dictionary<string, RedirectDictionary>());
    private RedirectsRepository _repository;

    public RedirectsRepositoryTests()
    {
        _config = new ContentfulConfig("unittest")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("UNITTEST_SPACE", "SPACE")
            .Add("UNITTEST_ACCESS_KEY", "KEY")
            .Add("UNITTEST_MANAGEMENT_KEY", "KEY")
            .Add("UNITTEST_ENVIRONMENT", "master")

            .Build();

        _createConfig
            .Setup(createConfig => createConfig(It.IsAny<string>()))
            .Returns(_config);

        _contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(_config))
            .Returns(_client.Object);

        _repository = new(_contentfulClientManager.Object,
                        _createConfig.Object,
                        new RedirectBusinessIds(new List<string> { "unittest" }),
                        _contenfulFactory.Object,
                        _shortUrlRedirects,
                        _legacyUrlRedirects);
    }

    [Fact]
    public async Task GetRedirects_ShouldGetsListOfRedirectsBack_ReturnSuccessful()
    {
        // Arrange
        ContentfulRedirect contentfulRedirects = new ContentfulRedirectBuilder().Build();
        ContentfulCollection<ContentfulRedirect> collection = new()
        {
            Items = new List<ContentfulRedirect> { contentfulRedirects }
        };

        BusinessIdToRedirects redirectItem = new(
            new Dictionary<string, string>
            {
                { "a-url", "another-url" }
            },
            new Dictionary<string, string>
            {
                { "some-url", "another-url" }
            });

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulRedirect>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _contenfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(contentfulRedirects))
            .Returns(redirectItem);

        // Act
        HttpResponse response = await _repository.GetRedirects();
        Redirects redirects = response.Get<Redirects>();

        // Assert
        Dictionary<string, RedirectDictionary> shortUrls = redirects.ShortUrlRedirects;
        Dictionary<string, RedirectDictionary> legacyUrls = redirects.LegacyUrlRedirects;

        Assert.Single(shortUrls);
        Assert.Single(legacyUrls);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("unittest", shortUrls.Keys.First());
        Assert.True(shortUrls["unittest"].ContainsKey("a-url"));
        Assert.Equal("unittest", legacyUrls.Keys.First());
        Assert.True(legacyUrls["unittest"].ContainsKey("some-url"));
    }

    [Fact]
    public async Task GetRedirects_BusinessIdExist_ShouldReturnSuccessful()
    {
        // Arrange
        ContentfulRedirect contentfulRedirects = new();
        ContentfulCollection<ContentfulRedirect> collection = new()
        {
            Items = new List<ContentfulRedirect>()
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulRedirect>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _contenfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(contentfulRedirects))
            .Returns(new NullBusinessIdToRedirects());

        // Act
        HttpResponse response = await _repository.GetRedirects();
        Redirects redirects = response.Get<Redirects>();

        // Assert
        Dictionary<string, RedirectDictionary> shortUrls = redirects.ShortUrlRedirects;
        Dictionary<string, RedirectDictionary> legacyUrls = redirects.LegacyUrlRedirects;

        Assert.Single(shortUrls);
        Assert.Single(legacyUrls);
        Assert.Empty(shortUrls["unittest"]);
        Assert.Empty(legacyUrls["unittest"]);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRedirects_NoBusinessId_ShouldReturnNotFound()
    {
        // Arrange
        ContentfulCollection<ContentfulRedirect> collection = new()
        {
            Items = new List<ContentfulRedirect>()
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulRedirect>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        RedirectsRepository repository = new(_contentfulClientManager.Object,
                                            _createConfig.Object,
                                            new RedirectBusinessIds(new List<string>()),
                                            _contenfulFactory.Object,
                                            _shortUrlRedirects,
                                            _legacyUrlRedirects);

        // Act
        HttpResponse response = await repository.GetRedirects();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetRedirect_StatusCodeSuccessful_WhenLegacyOrShortUrlAreAvailable()
    {
        // Arrange
        Dictionary<string, RedirectDictionary> shortItems = new() { { "unittest", new RedirectDictionary { { "/short-test", "short-redirect-url" } } } };
        Dictionary<string, RedirectDictionary> legacyItems = new() { { "unittest", new RedirectDictionary { { "/legacy-test", "legacy-redirect-url" } } } };
        _shortUrlRedirects.Redirects = shortItems;
        _legacyUrlRedirects.Redirects = legacyItems;

        // Act
        HttpResponse response = await _repository.GetRedirects();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRedirect_ShouldNotCallClient_WhenLegacyOrShortUrlAreAvailable()
    {
        // Arrange
        Dictionary<string, RedirectDictionary> shortItems = new() { { "unittest", new RedirectDictionary { { "/short-test", "short-redirect-url" } } } };
        Dictionary<string, RedirectDictionary> legacyItems = new() { { "unittest", new RedirectDictionary { { "/legacy-test", "legacy-redirect-url" } } } };
        _shortUrlRedirects.Redirects = shortItems;
        _legacyUrlRedirects.Redirects = legacyItems;

        // Act
        await _repository.GetRedirects();

        // Assert
        _client.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulRedirect>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetUpdatedRedirects_BusinessIdExist_ReturnSuccessful()
    {
        // Arrange
        ContentfulRedirect contentfulRedirects = new ContentfulRedirectBuilder().Build();
        ContentfulCollection<ContentfulRedirect> collection = new()
        {
            Items = new List<ContentfulRedirect> { contentfulRedirects }
        };

        BusinessIdToRedirects redirectItem = new(
            new Dictionary<string, string>
            {
                { "a-url", "another-url" }
            },
            new Dictionary<string, string>
            {
                { "some-url", "another-url" }
            });

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulRedirect>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _contenfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(contentfulRedirects))
            .Returns(redirectItem);

        // Act
        HttpResponse response = await _repository.GetUpdatedRedirects();
        Redirects redirects = response.Get<Redirects>();

        // Assert
        Dictionary<string, RedirectDictionary> shortUrls = redirects.ShortUrlRedirects;
        Dictionary<string, RedirectDictionary> legacyUrls = redirects.LegacyUrlRedirects;

        Assert.Single(shortUrls);
        Assert.Single(legacyUrls);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("unittest", shortUrls.Keys.First());
        Assert.True(shortUrls["unittest"].ContainsKey("a-url"));
        Assert.Equal("unittest", legacyUrls.Keys.First());
        Assert.True(legacyUrls["unittest"].ContainsKey("some-url"));
    }

    [Fact]
    public async Task GetUpdatedRedirects_NoBusinessId_ReturnNotFound()
    {
        // Arrange
        RedirectsRepository repository = new(_contentfulClientManager.Object,
                                            _createConfig.Object,
                                            new RedirectBusinessIds(new List<string>()),
                                            _contenfulFactory.Object,
                                            _shortUrlRedirects,
                                            _legacyUrlRedirects);

        // Act
        HttpResponse response = await repository.GetUpdatedRedirects();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}