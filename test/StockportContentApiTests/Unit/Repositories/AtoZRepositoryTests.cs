namespace StockportContentApiTests.Unit.Repositories;

public class AtoZRepositoryTests
{
    private readonly AtoZRepository _repository;
    private readonly Mock<IContentfulClient> _client = new();
    private readonly ContentfulConfig _config;
    private readonly Mock<IContentfulClientManager> _contentfulClientManager = new();
    private readonly Mock<IContentfulFactory<ContentfulAtoZ, AtoZ>> _aToZFactory = new();
    private readonly Mock<ICache> _cache = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<ILogger> _logger = new();

    public AtoZRepositoryTests()
    {
        _config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _configuration
            .Setup(conf => conf["redisExpiryTimes:AtoZ"])
            .Returns("60");

        _contentfulClientManager
            .Setup(client => client.GetClient(_config))
            .Returns(_client.Object);

        _repository = new(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);
    }

    [Fact]
    public void Get_ShouldReturnListOfAtoZForLetterV()
    {
        string letter = "v";
        List<AtoZ> aToZArticles = new()
        {
            new AtoZ("V atoztitle 1", "atozslug1", "atozteaser1", "article", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 2", "atozslug2", "atozteaser2", "article", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 3", "atozslug3", "atozteaser3", "article", new List<string> {"V atoztitle"})
        };

        List<AtoZ> aToZLandingPages = new()
        {
            new AtoZ("V atoztitle 1", "atozslug1", "atozteaser1", "landingPage", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 2", "atozslug2", "atozteaser2", "landingPage", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 3", "atozslug3", "atozteaser3", "landingPage", new List<string> {"V atoztitle"})
        };

        List<AtoZ> aToZTopics = new()
        {
            new AtoZ("V atoztitle 1", "atozslug1", "atozteaser1", "topic", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 2", "atozslug2", "atozteaser2", "topic", new List<string> {"V atoztitle"}),
            new AtoZ("V atoztitle 3", "atozslug3", "atozteaser3", "topic", new List<string> {"V atoztitle"})
        };

        _aToZFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("Vintage Village turns 6 years old",
                            "vintage-village-turns-6-years-old",
                            "The vintage village turned 6 with a great reception",
                            "article",
                            new List<string>()));
                
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-article-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(aToZArticles);
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-topic-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(aToZTopics);
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-showcase-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(aToZLandingPages);
    
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get(letter));
        List<AtoZ> aToZListing = response.Get<List<AtoZ>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(9, aToZListing.Count);
    }

    [Fact]
    public void Get_ShouldReturnListOfAtoZForLetterB()
    {
        string letter = "b";
        List<AtoZ> aToZArticles = new()
        {
            new AtoZ("B atoztitle 1", "atozslug1", "atozteaser1", "article", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 2", "atozslug2", "atozteaser2", "article", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 3", "atozslug3", "atozteaser3", "article", new List<string> {"V atoztitle"})
        };

        List<AtoZ> aToZLandingPages = new()
        {
            new AtoZ("B atoztitle 1", "atozslug1", "atozteaser1", "landingPage", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 2", "atozslug2", "atozteaser2", "landingPage", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 3", "atozslug3", "atozteaser3", "landingPage", new List<string> {"V atoztitle"})
        };

        List<AtoZ> aToZTopics = new()
        {
            new AtoZ("B atoztitle 1", "atozslug1", "atozteaser1", "topic", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 2", "atozslug2", "atozteaser2", "topic", new List<string> {"V atoztitle"}),
            new AtoZ("B atoztitle 3", "atozslug3", "atozteaser3", "topic", new List<string> {"V atoztitle"})
        };

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-article-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(aToZArticles);
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-topic-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(aToZTopics);
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-showcase-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(aToZLandingPages);

        _aToZFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("Bintage Village turns 6 years old",
                            "bintage-village-turns-6-years-old",
                            "The vintage village turned 6 with a great reception",
                            "article",
                            new List<string>()));

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get("b"));
        List<AtoZ> aToZListing = response.Get<List<AtoZ>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(9, aToZListing.Count);
    }
    

    [Theory]
    [InlineData("v")]
    [InlineData("b")]
    public void Get_ShouldReturnListOfAtoZForGivenLetter(string letter)
    {
        List<AtoZ> aToZArticles = new()
        {
            new AtoZ($"{letter.ToUpper()} atoztitle 1", "atozslug1", "atozteaser1", "article", new List<string> { $"{letter.ToUpper()} atoztitle" }),
            new AtoZ($"{letter.ToUpper()} atoztitle 2", "atozslug2", "atozteaser2", "article", new List<string> { $"{letter.ToUpper()} atoztitle" }),
            new AtoZ($"{letter.ToUpper()} atoztitle 3", "atozslug3", "atozteaser3", "article", new List<string> { $"{letter.ToUpper()} atoztitle" })
        };

        List<AtoZ> aToZLandingPages = new()
        {
            new AtoZ($"{letter.ToUpper()} atoztitle 1", "atozslug1", "atozteaser1", "landingPage", new List<string> { $"{letter.ToUpper()} atoztitle" }),
            new AtoZ($"{letter.ToUpper()} atoztitle 2", "atozslug2", "atozteaser2", "landingPage", new List<string> { $"{letter.ToUpper()} atoztitle" }),
            new AtoZ($"{letter.ToUpper()} atoztitle 3", "atozslug3", "atozteaser3", "landingPage", new List<string> { $"{letter.ToUpper()} atoztitle" })
        };

        List<AtoZ> aToZTopics = new()
        {
            new AtoZ($"{letter.ToUpper()} atoztitle 1", "atozslug1", "atozteaser1", "topic", new List<string> { $"{letter.ToUpper()} atoztitle" }),
            new AtoZ($"{letter.ToUpper()} atoztitle 2", "atozslug2", "atozteaser2", "topic", new List<string> { $"{letter.ToUpper()} atoztitle" }),
            new AtoZ($"{letter.ToUpper()} atoztitle 3", "atozslug3", "atozteaser3", "topic", new List<string> { $"{letter.ToUpper()} atoztitle" })
        };

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-article-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(aToZArticles);

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-topic-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(aToZTopics);

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-showcase-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(aToZLandingPages);

        _aToZFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ($"{letter}intage Village turns 6 years old",
                            $"{letter}intage-village-turns-6-years-old",
                            "The vintage village turned 6 with a great reception",
                            "article",
                            new List<string>()));

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get(letter));
        List<AtoZ> aToZListing = response.Get<List<AtoZ>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(9, aToZListing.Count);
    }



    [Fact]
    public void ItReturnsANotFoundIfNoItemsMatch()
    {
        // Arrange
        string letter = "b";
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(cacheKey => cacheKey.Equals($"test-atoz-article-{letter}")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.Is<int>(cacheTime => cacheTime.Equals(60))))
            .ReturnsAsync(new List<AtoZ>());
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-topic-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<AtoZ>());
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"test-atoz-showcase-{letter}")), It.IsAny<Func<Task<List<AtoZ>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<AtoZ>());

        _aToZFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("Vintage Village turns 6 years old",
                            "vintage-village-turns-6-years-old",
                            "The vintage village turned 6 with a great reception",
                            "article",
                            new List<string>()));

        AtoZRepository repository = new(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);

        HttpResponse response = AsyncTestHelper.Resolve(repository.Get("b"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No A to Z results found", response.Error);
    }

    [Fact]
    public async void ItGetsAnAtoZListingForTheLetterBWhereTheLetterMatchesWithAnAlterniveTitleAndSetsTheTitleAsTheAlternativeTitle()
    {
        // Arrange
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
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulAtoZ>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aToZcollection);

        _aToZFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("Because a Vintage Village turns 6 years old",
                            "vintage-village-turns-6-years-old",
                            "The vintage village turned 6 with a great reception",
                            "article",
                            new List<string> { alternativeTitle }));

        // Act
        List<AtoZ> aToZListing = await _repository.GetAtoZItemFromSource("article", "d");

        // Assert
        Assert.Equal(5, aToZListing.Count);
        Assert.Equal(alternativeTitle, aToZListing[0].Title);
    }

    [Fact]
    public void ItGetsAnAtoZListingItemWithMultipleAlternateTitles()
    {
        // Arrange
        List<string> alternateTitles = new() { "This is alternate title", "this is also another alternate title" };
        ContentfulCollection<ContentfulAtoZ> aToZcollection = new()
        {
            Items = new List<ContentfulAtoZ>
        {
            new ContentfulAToZBuilder().Title("Vintage Village").AlternativeTitles(alternateTitles).Build()
        }
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulAtoZ>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aToZcollection);

        _aToZFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("title", "slug", "teaser", "article", alternateTitles));

        // Act
        List<AtoZ> result = AsyncTestHelper.Resolve(_repository.GetAtoZItemFromSource("article", "t"));

        Assert.Equal(3, result.Count);
        Assert.Equal("title", result[0].Title);
        Assert.Equal("This is alternate title", result[1].Title);
        Assert.Equal("this is also another alternate title", result[2].Title);
    }
}