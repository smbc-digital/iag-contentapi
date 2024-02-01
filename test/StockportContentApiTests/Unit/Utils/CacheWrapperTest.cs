namespace StockportContentApiTests.Unit.Utils;

public class CacheTest
{
    readonly Cache _cacheWrapper;
    readonly Mock<IDistributedCacheWrapper> _distributedCacheWrapper;
    readonly Mock<ILogger<ICache>> _logger = new Mock<ILogger<ICache>>();

    public CacheTest()
    {
        _distributedCacheWrapper = new Mock<IDistributedCacheWrapper>();
        _cacheWrapper = new Cache(_distributedCacheWrapper.Object, _logger.Object, true);
    }

    [Fact]
    public void ShouldCallContentfulIfCacheIsEmpty()
    {
        // Arrange
        _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"), It.IsAny<CancellationToken>())).ReturnsAsync("");

        // Act
        var valueFromCall = _cacheWrapper.GetFromCacheOrDirectly("test-key", testFallbackMethod);

        // Assert
        valueFromCall.Should().Be("Contentful Data");
    }

    [Fact]
    public void ShouldNotCallContentfulIfCacheIsFull()
    {
        // Arrange
        _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"), It.IsAny<CancellationToken>())).ReturnsAsync("\"Cache Data\"");

        // Act
        var valueFromCall = _cacheWrapper.GetFromCacheOrDirectly("test-key", testFallbackMethod);

        // Assert
        valueFromCall.Should().Be("Cache Data");
    }

    private string testFallbackMethod() => "Contentful Data";    

    [Fact]
    public async void ShouldCallContentfulIfCacheIsEmptyAsync()
    {
        // Arrange
        _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"), It.IsAny<CancellationToken>())).ReturnsAsync("");

        // Act
        var valueFromCall = await _cacheWrapper.GetFromCacheOrDirectlyAsync("test-key", testFallbackMethodAsync);

        // Assert
        valueFromCall.Should().Be("Contentful Data");
        LogTesting.Assert(_logger, LogLevel.Information, "Key 'test-key' not found in cache of type: System.String");
    }

    [Fact]
    public async void ShouldNotCallContentfulIfCacheIsFullAsync()
    {
        // Arrange
        _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"), It.IsAny<CancellationToken>())).ReturnsAsync("\"Cache Data\"");

        // Act
        var valueFromCall = await _cacheWrapper.GetFromCacheOrDirectlyAsync("test-key", testFallbackMethodAsync);

        // Assert
        valueFromCall.Should().Be("Cache Data");
    }

    public async Task<string> testFallbackMethodAsync()
    {
        return await Task.FromResult("Contentful Data");
    }
}
