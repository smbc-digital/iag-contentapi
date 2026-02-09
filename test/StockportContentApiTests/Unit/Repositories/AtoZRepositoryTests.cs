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
    public async Task Get_ShouldReturnListOfAtoZ_WhenLetterIsV()
    {
        // Arrange
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
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(aToZArticles);
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(aToZTopics);
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(aToZLandingPages);
    
        // Act
        HttpResponse response = await _repository.Get(letter);
        List<AtoZ> aToZListing = response.Get<List<AtoZ>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(9, aToZListing.Count);
    }

    [Fact]
    public async Task Get_ShouldReturnListOfAtoZ_WhenLetterIsB()
    {
        // Arrange
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
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(aToZArticles);
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(aToZTopics);
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(aToZLandingPages);

        _aToZFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("Bintage Village turns 6 years old",
                            "bintage-village-turns-6-years-old",
                            "The vintage village turned 6 with a great reception",
                            "article",
                            new List<string>()));

        // Act
        HttpResponse response = await _repository.Get("b");
        List<AtoZ> aToZListing = response.Get<List<AtoZ>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(9, aToZListing.Count);
    }
    
    [Theory]
    [InlineData("v")]
    [InlineData("b")]
    public async Task Get_ShouldReturnListOfAtoZForGivenLetter(string letter)
    {
        // Arrange
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
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(aToZArticles);

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(aToZTopics);

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(aToZLandingPages);

        _aToZFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ($"{letter}intage Village turns 6 years old",
                            $"{letter}intage-village-turns-6-years-old",
                            "The vintage village turned 6 with a great reception",
                            "article",
                            new List<string>()));

        // Act
        HttpResponse response = await _repository.Get(letter);
        List<AtoZ> aToZListing = response.Get<List<AtoZ>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(9, aToZListing.Count);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFoundIfNoItemsMatch()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(),
                                                            It.IsAny<int>()))
            .ReturnsAsync(new List<AtoZ>());
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<AtoZ>());
        
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<AtoZ>());

        _aToZFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()))
            .Returns(new AtoZ("Vintage Village turns 6 years old",
                            "vintage-village-turns-6-years-old",
                            "The vintage village turned 6 with a great reception",
                            "article",
                            new List<string>()));

        AtoZRepository repository = new(_config, _contentfulClientManager.Object, _aToZFactory.Object, null, _cache.Object, _configuration.Object, _logger.Object);

        // Act
        HttpResponse response = await repository.Get("b");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No A to Z results found", response.Error);
        _aToZFactory.Verify(factory => factory.ToModel(It.IsAny<ContentfulAtoZ>()), Times.Never);
    }

    [Fact]
    public async Task GetAtoZItemFromSource_ShouldReturnItemsUsingAlternativeTitleWhenItMatchesTheSearchLetter()
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
        List<AtoZ> aToZListing = await _repository.GetAtoZItemFromSource("article", "d", "tagId");

        // Assert
        Assert.Equal(5, aToZListing.Count);
        Assert.Equal(alternativeTitle, aToZListing[0].Title);
        Assert.All(aToZListing, item => Assert.Equal(alternativeTitle, item.Title));
    }

    [Fact]
    public async Task GetAtoZItemFromSource_ShouldReturnAnAtoZListingItemWithMultipleAlternateTitles()
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
        List<AtoZ> result = await _repository.GetAtoZItemFromSource("article", "t");

        Assert.Equal(3, result.Count);
        Assert.Equal("title", result[0].Title);
        Assert.Equal("This is alternate title", result[1].Title);
        Assert.Equal("this is also another alternate title", result[2].Title);
        Assert.All(result, item => Assert.Equal("slug", item.Slug));
        Assert.All(result, item => Assert.Equal("article", item.Type));
    }

    [Fact]
    public async Task Get_ShouldReturnAllAtoZItemsWhenNoLetterProvided()
    {
        // Arrange
        AtoZ atozItem1 = new("Apple", "apple", "teaser apple", "article", new List<string>());
        AtoZ atozItem2 = new("Banana", "banana", "teaser banana", "topic", new List<string>());
        AtoZ atozItem3 = new("Zebra", "zebra", "teaser zebra", "landingPage", new List<string>());

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(key => key.Contains("article")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<AtoZ> { atozItem1 });

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(key => key.Contains("topic")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<AtoZ> { atozItem2 });

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.Is<string>(key => key.Contains("landingPage")),
                                                            It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<AtoZ> { atozItem3 });

        // Act
        HttpResponse response = await _repository.Get("tagId");

        // Assert
        List<AtoZ> result = response.Get<List<AtoZ>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("apple", result[0].Slug);
        Assert.Equal("banana", result[1].Slug);
        Assert.Equal("zebra", result[2].Slug);
    }
}