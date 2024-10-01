namespace StockportContentApiTests.Unit.Repositories;

public class RedirectsRepositoryTests
{
    private readonly ContentfulConfig _config;
    private readonly Mock<Func<string, ContentfulConfig>> _createConfig;
    private readonly Mock<IContentfulClientManager> _contentfulClientManager;
    private readonly Mock<IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>> _contenfulFactory;
    private readonly Mock<IContentfulClient> _client;
    private readonly ShortUrlRedirects _shortUrlRedirects = new(new Dictionary<string, RedirectDictionary>());
    private readonly LegacyUrlRedirects _legacyUrlRedirects = new(new Dictionary<string, RedirectDictionary>());

    public RedirectsRepositoryTests()
    {
        _config = new ContentfulConfig("unittest")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("UNITTEST_SPACE", "SPACE")
            .Add("UNITTEST_ACCESS_KEY", "KEY")
            .Add("UNITTEST_MANAGEMENT_KEY", "KEY")
            .Add("UNITTEST_ENVIRONMENT", "master")

            .Build();

        _createConfig = new Mock<Func<string, ContentfulConfig>>();
        _contenfulFactory = new Mock<IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>>();
        _contentfulClientManager = new Mock<IContentfulClientManager>();
        _client = new Mock<IContentfulClient>();
        _createConfig.Setup(o => o(It.IsAny<string>())).Returns(_config);
        _contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);
    }

    [Fact]
    public void GetRedirects_ShouldGetsListOfRedirectsBack_ReturnSuccessful()
    {
        ContentfulRedirect ContentfulRedirects = new ContentfulRedirectBuilder().Build();
        ContentfulCollection<ContentfulRedirect> collection = new()
        {
            Items = new List<ContentfulRedirect> { ContentfulRedirects }
        };

        BusinessIdToRedirects redirectItem = new(new Dictionary<string, string> { { "a-url", "another-url" } }, new Dictionary<string, string> { { "some-url", "another-url" } });

        QueryBuilder<ContentfulRedirect> builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        RedirectsRepository repository = new(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);
        _contenfulFactory.Setup(o => o.ToModel(ContentfulRedirects)).Returns(redirectItem);

        HttpResponse response = AsyncTestHelper.Resolve(repository.GetRedirects());

        Redirects redirects = response.Get<Redirects>();

        Dictionary<string, RedirectDictionary> shortUrls = redirects.ShortUrlRedirects;
        shortUrls.Count.Should().Be(1);
        shortUrls.Keys.First().Should().Be("unittest");
        shortUrls["unittest"].ContainsKey("a-url").Should().BeTrue();
        Dictionary<string, RedirectDictionary> legacyUrls = redirects.LegacyUrlRedirects;
        legacyUrls.Count.Should().Be(1);
        legacyUrls.Keys.First().Should().Be("unittest");
        legacyUrls["unittest"].ContainsKey("some-url").Should().BeTrue();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void GetRedirects_BusinessIdExist_ShouldReturnSuccessful()
    {
        ContentfulRedirect ContentfulRedirects = new();
        ContentfulCollection<ContentfulRedirect> collection = new()
        {
            Items = new List<ContentfulRedirect>()
        };

        QueryBuilder<ContentfulRedirect> builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        RedirectsRepository repository = new(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);
        _contenfulFactory.Setup(o => o.ToModel(ContentfulRedirects)).Returns(new NullBusinessIdToRedirects());

        HttpResponse response = AsyncTestHelper.Resolve(repository.GetRedirects());

        Redirects redirects = response.Get<Redirects>();

        Dictionary<string, RedirectDictionary> shortUrls = redirects.ShortUrlRedirects;
        shortUrls.Count.Should().Be(1);
        shortUrls["unittest"].Count.Should().Be(0);
        Dictionary<string, RedirectDictionary> legacyUrls = redirects.LegacyUrlRedirects;
        legacyUrls.Count.Should().Be(1);
        legacyUrls["unittest"].Count.Should().Be(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }

    [Fact]
    public void GetRedirects_NoBusinessId_ShouldReturnNotFound()
    {
        ContentfulCollection<ContentfulRedirect> collection = new()
        {
            Items = new List<ContentfulRedirect>()
        };

        QueryBuilder<ContentfulRedirect> builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        RedirectsRepository repository = new(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string>()), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);

        HttpResponse response = AsyncTestHelper.Resolve(repository.GetRedirects());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void GetRedirect_StatusCodeSuccessful_WhenLegacyOrShortUrlAreAvailable()
    {
        Dictionary<string, RedirectDictionary> shortItems = new() { { "unittest", new RedirectDictionary { { "/short-test", "short-redirect-url" } } } };
        Dictionary<string, RedirectDictionary> legacyItems = new() { { "unittest", new RedirectDictionary { { "/legacy-test", "legacy-redirect-url" } } } };
        _shortUrlRedirects.Redirects = shortItems;
        _legacyUrlRedirects.Redirects = legacyItems;

        RedirectsRepository repository = new(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);

        HttpResponse response = AsyncTestHelper.Resolve(repository.GetRedirects());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void GetRedirect_ShouldNotCallClient_WhenLegacyOrShortUrlAreAvailable()
    {
        Dictionary<string, RedirectDictionary> shortItems = new() { { "unittest", new RedirectDictionary { { "/short-test", "short-redirect-url" } } } };
        Dictionary<string, RedirectDictionary> legacyItems = new() { { "unittest", new RedirectDictionary { { "/legacy-test", "legacy-redirect-url" } } } };
        _shortUrlRedirects.Redirects = shortItems;
        _legacyUrlRedirects.Redirects = legacyItems;

        RedirectsRepository repository = new(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);

        AsyncTestHelper.Resolve(repository.GetRedirects());
        QueryBuilder<ContentfulRedirect> builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

        _client.Verify(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void GetUpdatedRedirects_BusinessIdExist_ReturnSuccessful()
    {
        ContentfulRedirect ContentfulRedirects = new ContentfulRedirectBuilder().Build();
        ContentfulCollection<ContentfulRedirect> collection = new()
        {
            Items = new List<ContentfulRedirect> { ContentfulRedirects }
        };

        BusinessIdToRedirects redirectItem = new(new Dictionary<string, string> { { "a-url", "another-url" } }, new Dictionary<string, string> { { "some-url", "another-url" } });

        QueryBuilder<ContentfulRedirect> builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("redirect").Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulRedirect>>(q => q.Build() == builder.Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        RedirectsRepository repository = new(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string> { "unittest" }), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);
        _contenfulFactory.Setup(o => o.ToModel(ContentfulRedirects)).Returns(redirectItem);

        HttpResponse response = AsyncTestHelper.Resolve(repository.GetUpdatedRedirects());

        Redirects redirects = response.Get<Redirects>();

        Dictionary<string, RedirectDictionary> shortUrls = redirects.ShortUrlRedirects;
        shortUrls.Count.Should().Be(1);
        shortUrls.Keys.First().Should().Be("unittest");
        shortUrls["unittest"].ContainsKey("a-url").Should().BeTrue();
        Dictionary<string, RedirectDictionary> legacyUrls = redirects.LegacyUrlRedirects;
        legacyUrls.Count.Should().Be(1);
        legacyUrls.Keys.First().Should().Be("unittest");
        legacyUrls["unittest"].ContainsKey("some-url").Should().BeTrue();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void GetUpdatedRedirects_NoBusinessId_ReturnNotFound()
    {
        RedirectsRepository repository = new(_contentfulClientManager.Object, _createConfig.Object, new RedirectBusinessIds(new List<string>()), _contenfulFactory.Object, _shortUrlRedirects, _legacyUrlRedirects);

        HttpResponse response = AsyncTestHelper.Resolve(repository.GetUpdatedRedirects());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}