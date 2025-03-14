namespace StockportContentApiTests.Unit.Repositories;

public class AtoZRepositoryTests
{
    private readonly Mock<IContentfulClient> _client;
    private readonly ContentfulConfig _config;
    private readonly Mock<IContentfulClientManager> _contentfulClientManager;
    private readonly Mock<IContentfulFactory<ContentfulAtoZ, AtoZ>> _aToZFactory;
    private readonly Mock<ICache> _cache;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<ILogger> _logger;

    public AtoZRepositoryTests()
    {
        _config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(_ => _["redisExpiryTimes:AtoZ"]).Returns("60");
        _cache = new Mock<ICache>();

        _contentfulClientManager = new Mock<IContentfulClientManager>();
        _client = new Mock<IContentfulClient>();
        _contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);
        _aToZFactory = new Mock<IContentfulFactory<ContentfulAtoZ, AtoZ>>();
        _logger = new Mock<ILogger>();
    }

    [Fact]
    public void ItGetsAnAtoZListingForTheLetterV()
    {
        string letter = "v";
        List<AtoZ> aToZArticle = new()
        {
            new AtoZ("V atoztitle 1", "atozslug1", "atozteaser1", "article", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 2", "atozslug2", "atozteaser2", "article", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 3", "atozslug3", "atozteaser3", "article", new List<string> {"V atoztitle"})
        };

        List<AtoZ> aToZShowcase = new()
        {
            new AtoZ("V atoztitle 1", "atozslug1", "atozteaser1", "showcase", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 2", "atozslug2", "atozteaser2", "showcase", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 3", "atozslug3", "atozteaser3", "showcase", new List<string> {"V atoztitle"})
        };

        List<AtoZ> aToZTopic = new()
        {
            new AtoZ("V atoztitle 1", "atozslug1", "atozteaser1", "topic", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 2", "atozslug2", "atozteaser2", "topic", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 3", "atozslug3", "atozteaser3", "topic", new List<string> {"V atoztitle"})
        };

        _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("Vintage Village turns 6 years old", "vintage-village-turns-6-years-old",
                "The vintage village turned 6 with a great reception", "article", new List<string>()));
                
        AtoZRepository repository = new(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);

        _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-article-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(aToZArticle);
        _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-topic-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(aToZTopic);
        _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-showcase-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(aToZShowcase);
    
        HttpResponse response = AsyncTestHelper.Resolve(repository.Get(letter));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<AtoZ> aToZListing = response.Get<List<AtoZ>>();
        aToZListing.Count.Should().Be(9);
    }

    [Fact]
    public void ItGetsAnAtoZListingForTheLetterB()
    {
        string letter = "b";
        List<AtoZ> aToZArticle = new()
        {
            new AtoZ("B atoztitle 1", "atozslug1", "atozteaser1", "article", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 2", "atozslug2", "atozteaser2", "article", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 3", "atozslug3", "atozteaser3", "article", new List<string> {"V atoztitle"})
        };

        List<AtoZ> aToZShowcase = new()
        {
            new AtoZ("B atoztitle 1", "atozslug1", "atozteaser1", "showcase", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 2", "atozslug2", "atozteaser2", "showcase", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 3", "atozslug3", "atozteaser3", "showcase", new List<string> {"V atoztitle"})
        };

        List<AtoZ> aToZTopic = new()
        {
            new AtoZ("B atoztitle 1", "atozslug1", "atozteaser1", "topic", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 2", "atozslug2", "atozteaser2", "topic", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 3", "atozslug3", "atozteaser3", "topic", new List<string> {"V atoztitle"})
        };

        _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-article-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(aToZArticle);
        _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-topic-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(aToZTopic);
        _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-showcase-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(aToZShowcase);

        _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
           .Returns(new AtoZ("Bintage Village turns 6 years old", "bintage-village-turns-6-years-old",
               "The vintage village turned 6 with a great reception", "article", new List<string>()));

        AtoZRepository repository = new(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);
        HttpResponse response = AsyncTestHelper.Resolve(repository.Get("b"));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<AtoZ> aToZListing = response.Get<List<AtoZ>>();

        aToZListing.Count.Should().Be(9);
    }

    [Fact]
    public void ItReturnsANotFoundIfNoItemsMatch()
    {
        string letter = "b";
        _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-article-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(new List<AtoZ>());
        _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-topic-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(new List<AtoZ>());
        _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-showcase-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60)))).ReturnsAsync(new List<AtoZ>());

        _aToZFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
           .Returns(new AtoZ("Vintage Village turns 6 years old", "vintage-village-turns-6-years-old",
               "The vintage village turned 6 with a great reception", "article", new List<string>()));

        AtoZRepository repository = new(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);

        HttpResponse response = AsyncTestHelper.Resolve(repository.Get("b"));
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Error.Should().Be("No results found");
    }

    [Fact]
    public async void ItGetsAnAtoZListingForTheLetterBWhereTheLetterMatchesWithAnAlterniveTitleAndSetsTheTitleAsTheAlternativeTitle()
    {
        string alternativeTitle = "Do you know this started!";
        ContentfulCollection<ContentfulAtoZ> aToZcollection = new()
        {
            Items = new List<ContentfulAtoZ>
        {
            new ContentfulAToZBuilder().Title("Vintage Village 1").AlternativeTitles(new List<string> { alternativeTitle }).Build(),
            new ContentfulAToZBuilder().Title("Vintage Village 2").AlternativeTitles(new List<string> { alternativeTitle }).Build(),
            new ContentfulAToZBuilder().Title("Vintage Village 3").AlternativeTitles(new List<string> { alternativeTitle }).Build(),
            new ContentfulAToZBuilder().Title("Vintage Village 4").AlternativeTitles(new List<string> { alternativeTitle }).Build(),
            new ContentfulAToZBuilder().Title("Vintage Village 5").AlternativeTitles(new List<string> { alternativeTitle }).Build()
        }
        };

        _client
            .Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulAtoZ>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aToZcollection);

        _aToZFactory
            .Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("Because a Vintage Village turns 6 years old", "vintage-village-turns-6-years-old", "The vintage village turned 6 with a great reception", "article", new List<string> { alternativeTitle }));

        AtoZRepository repository = new(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);
        List<AtoZ> aToZListing = await repository.GetAtoZItemFromSource("article", "d");

        aToZListing.Count.Should().Be(5);
        aToZListing.FirstOrDefault().Title.Should().Be(alternativeTitle);
    }

    [Fact]
    public void ItGetsAnAtoZListingItemWithMultipleAlternateTitles()
    {
        List<string> alternateTitles = new() { "This is alternate title", "this is also another alternate title" };
        ContentfulCollection<ContentfulAtoZ> aToZcollection = new()
        {
            Items = new List<ContentfulAtoZ>
        {
            new ContentfulAToZBuilder().Title("Vintage Village").AlternativeTitles(alternateTitles).Build()
        }
        };

        _client
            .Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulAtoZ>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aToZcollection);

        _aToZFactory
            .Setup(o => o.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("title", "slug", "teaser", "article", alternateTitles));
        AtoZRepository repository = new(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);

        List<AtoZ> result = AsyncTestHelper.Resolve(repository.GetAtoZItemFromSource("article", "t"));

        result.Count.Should().Be(3);
        result[0].Title.Should().Be("title");
        result[1].Title.Should().Be("This is alternate title");
        result[2].Title.Should().Be("this is also another alternate title");
    }
}
