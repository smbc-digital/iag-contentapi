namespace StockportContentApiTests.Unit.Repositories;

public class CommsRepositoryTests
{
    private readonly Mock<IContentfulClientManager> _clientManager = new();
    private readonly Mock<IContentfulClient> _client = new();
    private readonly Mock<IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>> _commsHomepageFactory = new();
    private readonly CommsRepository _repository;

    public CommsRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _clientManager
            .Setup(clientManager => clientManager.GetClient(It.IsAny<ContentfulConfig>()))
            .Returns(_client.Object);

        _repository = new CommsRepository(config, _clientManager.Object, _commsHomepageFactory.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnCommsHomepageModel()
    {
        // Arrange
        _commsHomepageFactory
            .Setup(commsHomepageFactory => commsHomepageFactory.ToModel(It.IsAny<ContentfulCommsHomepage>()))
            .Returns(new CommsHomepage(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                new List<BasicLink>(),
                new EventBuilder().Build(),
                new CallToActionBanner(),
                string.Empty
            ));

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulCommsHomepage>
                {
                    Items = new List<ContentfulCommsHomepage>
                    {
                        new() {
                            WhatsOnInStockportEvent = new ContentfulEventBuilder().Build()
                        }
                    }
                });

        // Act
        HttpResponse result = await _repository.Get();
        CommsHomepage parsedResult = result.Get<CommsHomepage>();

        // Assert
        _commsHomepageFactory.Verify(commsHomepageFactory => commsHomepageFactory.ToModel(It.IsAny<ContentfulCommsHomepage>()), Times.Once);
        _client.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsType<CommsHomepage>(parsedResult);
    }

    [Fact]
    public async Task Get_ShouldReturnCommsHomepageModel_AndNoEvent()
    {
        // Arrange
        ContentfulCommsHomepage commsCallback = new();

        _commsHomepageFactory
            .Setup(commsHomepageFactory => commsHomepageFactory.ToModel(It.IsAny<ContentfulCommsHomepage>()))
            .Returns(new CommsHomepage(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                new List<BasicLink>(),
                null,
                new CallToActionBanner(),
                string.Empty
            ))
            .Callback<ContentfulCommsHomepage>(x => commsCallback = x);

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulCommsHomepage>
                {
                    Items = new List<ContentfulCommsHomepage>
                    {
                        new()
                    }
                });

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulEvent>
                {
                    Items = new List<ContentfulEvent>()
                });

        // Act
        HttpResponse result = await _repository.Get();
        CommsHomepage parsedResult = result.Get<CommsHomepage>();

        // Assert
        _commsHomepageFactory.Verify(commsHomepageFactory => commsHomepageFactory.ToModel(It.IsAny<ContentfulCommsHomepage>()), Times.Once);
        _client.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
        _client.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsType<CommsHomepage>(parsedResult);
        Assert.Null(commsCallback.WhatsOnInStockportEvent);
    }

    [Fact]
    public async Task Get_ShouldReturnCommsHomepageModel_AndHasEvent()
    {
        // Arrange
        ContentfulEvent expectedEvent = new ContentfulEventBuilder().Build();
        ContentfulCommsHomepage commsCallback = new();

        _commsHomepageFactory
            .Setup(commsHomepageFactory => commsHomepageFactory.ToModel(It.IsAny<ContentfulCommsHomepage>()))
            .Returns(new CommsHomepage(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                new List<BasicLink>(),
                null,
                new CallToActionBanner(),
                string.Empty
            ))
            .Callback<ContentfulCommsHomepage>(contentfulCommsHomepage => commsCallback = contentfulCommsHomepage);

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulCommsHomepage>
                {
                    Items = new List<ContentfulCommsHomepage>
                    {
                        new()
                    }
                });

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulEvent>
                {
                    Items = new List<ContentfulEvent>
                    {
                        expectedEvent
                    }
                });

        // Act
        HttpResponse result = await _repository.Get();
        CommsHomepage parsedResult = result.Get<CommsHomepage>();

        // Assert
        _commsHomepageFactory.Verify(commsHomepageFactory => commsHomepageFactory.ToModel(It.IsAny<ContentfulCommsHomepage>()), Times.Once);
        _client.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
        _client.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsType<CommsHomepage>(parsedResult);
        Assert.NotNull(commsCallback.WhatsOnInStockportEvent);
        Assert.Same(expectedEvent, commsCallback.WhatsOnInStockportEvent);
    }

    [Fact]
    public async Task Get_ShouldReturnStatus500()
    {
        // Arrange
        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulCommsHomepage>
                {
                    Items = new List<ContentfulCommsHomepage>()
                });

        // Act
        HttpResponse result = await _repository.Get();

        // Assert
        _client.Verify(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
        _commsHomepageFactory.VerifyNoOtherCalls();
        _client.VerifyNoOtherCalls();
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal("No comms homepage found", result.Error);
    }
}