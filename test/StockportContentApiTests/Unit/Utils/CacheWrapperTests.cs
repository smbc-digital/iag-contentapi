namespace StockportContentApiTests.Unit.Utils;

public class CacheTests
{
    private readonly Cache _cacheWrapper;
    private readonly Mock<IDistributedCacheWrapper> _distributedCacheWrapper = new();
    private readonly Mock<ILogger<ICache>> _logger = new();

    public CacheTests() =>
        _cacheWrapper = new(_distributedCacheWrapper.Object, _logger.Object, true);

    [Fact]
    public void ShouldCallContentfulIfCacheIsEmpty()
    {
        // Arrange
        _distributedCacheWrapper
            .Setup(distributedCacheWrapper => distributedCacheWrapper.GetString(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        // Act
        string result = _cacheWrapper.GetFromCacheOrDirectly("test-key", TestFallbackMethod);

        // Assert
        Assert.Equal("Contentful Data", result);
    }

    [Fact]
    public void ShouldNotCallContentfulIfCacheIsFull()
    {
        // Arrange
        _distributedCacheWrapper
            .Setup(distributedCacheWrapper => distributedCacheWrapper.GetString(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("\"Cache Data\"");

        // Act
        string result = _cacheWrapper.GetFromCacheOrDirectly("test-key", TestFallbackMethod);

        // Assert
        Assert.Equal("Cache Data", result);
    }

    private string TestFallbackMethod() =>
        "Contentful Data";

    [Fact]
    public async Task ShouldCallContentfulIfCacheIsEmptyAsync()
    {
        // Arrange
        _distributedCacheWrapper
            .Setup(distributedCacheWrapper => distributedCacheWrapper.GetString(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        // Act
        string result = await _cacheWrapper.GetFromCacheOrDirectlyAsync("test-key", TestFallbackMethodAsync);

        // Assert
        Assert.Equal("Contentful Data", result);
        LogTesting.Assert(_logger, LogLevel.Information, "Key 'test-key' not found in cache of type: System.String");
    }

    [Fact]
    public async Task ShouldNotCallContentfulIfCacheIsFullAsync()
    {
        // Arrange
        _distributedCacheWrapper
            .Setup(distributedCacheWrapper => distributedCacheWrapper.GetString(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("\"Cache Data\"");

        // Act
        string result = await _cacheWrapper.GetFromCacheOrDirectlyAsync("test-key", TestFallbackMethodAsync);

        // Assert
        Assert.Equal("Cache Data", result);
    }

    public async Task<string> TestFallbackMethodAsync() =>
        await Task.FromResult("Contentful Data");

    [Fact]
    public async Task ShouldOnlyCallFallbackOnceForConcurrentRequestsToSameKey()
    {
        string cachedData = string.Empty;
        int fallbackCalls = 0;

        _distributedCacheWrapper
            .Setup(distributedCacheWrapper => distributedCacheWrapper.GetString(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => cachedData);
        _distributedCacheWrapper
            .Setup(distributedCacheWrapper => distributedCacheWrapper.SetString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .Callback<string, string, int>((_, value, _) => cachedData = value);

        async Task<string> Fallback()
        {
            Interlocked.Increment(ref fallbackCalls);
            await Task.Delay(50);
            return "Contentful Data";
        }

        Task<string>[] requests = Enumerable.Range(0, 10)
            .Select(_ => _cacheWrapper.GetFromCacheOrDirectlyWithLockingAsync("same-key", Fallback, 60))
            .ToArray();

        string[] results = await Task.WhenAll(requests);

        Assert.All(results, result => Assert.Equal("Contentful Data", result));
        Assert.Equal(1, fallbackCalls);
    }
}