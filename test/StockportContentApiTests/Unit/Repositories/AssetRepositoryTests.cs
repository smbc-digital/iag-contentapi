namespace StockportContentApiTests.Unit.Repositories;

public class AssetRepositoryTests
{
    private readonly Mock<IContentfulClientManager> _contentfulClientManager = new();
    private readonly Mock<ILogger<AssetRepository>> _logger = new();
    private readonly Mock<IContentfulClient> _contentfulClient = new();

    public AssetRepositoryTests()
    {
        _contentfulClientManager.Setup(o => o.GetClient(It.IsAny<ContentfulConfig>()))
            .Returns(_contentfulClient.Object);
    }

    [Fact]
    public async void ShouldReturnAsset()
    {
        _contentfulClient.Setup(o => o.GetAsset("asset", It.IsAny<QueryBuilder<Asset>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Asset());

        AssetRepository assetRepository = new(new ContentfulConfig(string.Empty, string.Empty, string.Empty), _contentfulClientManager.Object,
            _logger.Object);

        Asset asset = await assetRepository.Get("asset");

        asset.Should().NotBeNull();
    }

    [Fact]
    public async void ShouldReturnNullIfNoAssetIsFoundAndLogWarning()
    {
        _contentfulClient.Setup(o =>
                o.GetAsset("asset-fail", It.IsAny<QueryBuilder<Asset>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ContentfulException(500, "There was a problem with getting assetid: asset-fail from contentful"));

        AssetRepository assetRepository = new(new ContentfulConfig(string.Empty, string.Empty, string.Empty), _contentfulClientManager.Object,
            _logger.Object);

        await assetRepository.Get("asset-fail");

        LogTesting.Assert(_logger, LogLevel.Warning, "There was a problem with getting assetId: asset-fail from contentful");
    }
}
