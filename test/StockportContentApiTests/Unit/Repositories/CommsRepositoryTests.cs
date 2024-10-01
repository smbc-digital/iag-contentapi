namespace StockportContentApiTests.Unit.Repositories;

public class CommsRepositoryTests
{
    private readonly Mock<IContentfulClientManager> _mockClientManager = new();
    private readonly Mock<IContentfulClient> _mockClient = new();
    private readonly Mock<IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>> _mockCommsHomepageFactory = new();
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

        _mockClientManager.Setup(_ => _.GetClient(It.IsAny<ContentfulConfig>())).Returns(_mockClient.Object);

        _repository = new CommsRepository(config, _mockClientManager.Object, _mockCommsHomepageFactory.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnCommsHomepageModel()
    {
        // Arrange
        _mockCommsHomepageFactory
            .Setup(_ => _
                .ToModel(It.IsAny<ContentfulCommsHomepage>()))
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

        _mockClient
            .Setup(_ =>
                _.GetEntries(
                    It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(),
                    It.IsAny<CancellationToken>()))
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
        _mockCommsHomepageFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulCommsHomepage>()), Times.Once);
        _mockClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsType<CommsHomepage>(parsedResult);
    }

    [Fact]
    public async Task Get_ShouldReturnCommsHomepageModel_AndNoEvent()
    {
        // Arrange
        ContentfulCommsHomepage commsCallback = new();

        _mockCommsHomepageFactory
            .Setup(_ => _
                .ToModel(It.IsAny<ContentfulCommsHomepage>()))
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

        _mockClient
            .Setup(_ =>
                _.GetEntries(
                    It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulCommsHomepage>
                {
                    Items = new List<ContentfulCommsHomepage>
                    {
                        new()
                    }
                });

        _mockClient
            .Setup(_ =>
                _.GetEntries(
                    It.IsAny<QueryBuilder<ContentfulEvent>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulEvent>
                {
                    Items = new List<ContentfulEvent>()
                });

        // Act
        HttpResponse result = await _repository.Get();
        CommsHomepage parsedResult = result.Get<CommsHomepage>();

        // Assert
        _mockCommsHomepageFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulCommsHomepage>()), Times.Once);
        _mockClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
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

        _mockCommsHomepageFactory
            .Setup(_ => _
                .ToModel(It.IsAny<ContentfulCommsHomepage>()))
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

        _mockClient
            .Setup(_ =>
                _.GetEntries(
                    It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulCommsHomepage>
                {
                    Items = new List<ContentfulCommsHomepage>
                    {
                        new()
                    }
                });

        _mockClient
            .Setup(_ =>
                _.GetEntries(
                    It.IsAny<QueryBuilder<ContentfulEvent>>(),
                    It.IsAny<CancellationToken>()))
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
        _mockCommsHomepageFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulCommsHomepage>()), Times.Once);
        _mockClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsType<CommsHomepage>(parsedResult);
        Assert.NotNull(commsCallback.WhatsOnInStockportEvent);
        Assert.Same(expectedEvent, commsCallback.WhatsOnInStockportEvent);
    }

    [Fact]
    public async Task Get_ShouldReturnStatus500()
    {
        // Arrange
        _mockClient
            .Setup(_ =>
                _.GetEntries(
                    It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ContentfulCollection<ContentfulCommsHomepage>
                {
                    Items = new List<ContentfulCommsHomepage>()
                });

        // Act
        HttpResponse result = await _repository.Get();

        // Assert
        _mockClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulCommsHomepage>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockCommsHomepageFactory.VerifyNoOtherCalls();
        _mockClient.VerifyNoOtherCalls();
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal("No comms homepage found", result.Error);
    }
}
