namespace StockportContentApiTests.Unit.Repositories;

public class AssetRepositoryTests
{
    private readonly Mock<IContentfulClientManager> _contentfulClientManager = new();
    private readonly Mock<ILogger<AssetRepository>> _logger = new();
    private readonly Mock<IContentfulClient> _contentfulClient = new();

    public AssetRepositoryTests() =>
        _contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(It.IsAny<ContentfulConfig>()))
            .Returns(_contentfulClient.Object);

    [Fact]
    public async Task ShouldReturnAsset()
    {
        // Arrange
        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetAsset("asset", It.IsAny<QueryBuilder<Asset>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Asset());

        AssetRepository assetRepository = new(new ContentfulConfig(string.Empty, string.Empty, string.Empty),
                                            _contentfulClientManager.Object,
                                            _logger.Object);

        // Act
        Asset asset = await assetRepository.Get("asset");

        // Assert
        Assert.NotNull(asset);
    }

    [Fact]
    public async Task ShouldReturnNullIfNoAssetIsFoundAndLogWarning()
    {
        // Arrange
        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetAsset("asset-fail", It.IsAny<QueryBuilder<Asset>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ContentfulException(500, "There was a problem with getting assetid: asset-fail from contentful"));

        AssetRepository assetRepository = new(new ContentfulConfig(string.Empty, string.Empty, string.Empty),
                                            _contentfulClientManager.Object,
                                            _logger.Object);

        // Act
        await assetRepository.Get("asset-fail");

        // Assert
        LogTesting.Assert(_logger, LogLevel.Warning, "There was a problem with getting assetId: asset-fail from contentful");
    }
}