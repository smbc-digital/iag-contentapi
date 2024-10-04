namespace StockportContentApiTests.Unit.Utils;

public class CacheTests
{
    private readonly Cache _cacheWrapper;
    private readonly Mock<IDistributedCacheWrapper> _distributedCacheWrapper;
    private readonly Mock<ILogger<ICache>> _logger = new();

    public CacheTests()
    {
        _distributedCacheWrapper = new();
        _cacheWrapper = new(_distributedCacheWrapper.Object, _logger.Object, true);
    }

    [Fact]
    public void ShouldCallContentfulIfCacheIsEmpty()
    {
        // Arrange
        _distributedCacheWrapper
            .Setup(o => o.GetString(It.Is<string>(s => s.Equals("test-key")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        // Act
        string valueFromCall = _cacheWrapper.GetFromCacheOrDirectly("test-key", testFallbackMethod);

        // Assert
        valueFromCall.Should().Be("Contentful Data");
    }

    [Fact]
    public void ShouldNotCallContentfulIfCacheIsFull()
    {
        // Arrange
        _distributedCacheWrapper
            .Setup(o => o.GetString(It.Is<string>(s => s.Equals("test-key")), It.IsAny<CancellationToken>()))
            .ReturnsAsync("\"Cache Data\"");

        // Act
        string valueFromCall = _cacheWrapper.GetFromCacheOrDirectly("test-key", testFallbackMethod);

        // Assert
        valueFromCall.Should().Be("Cache Data");
    }

    private string testFallbackMethod() => "Contentful Data";

    [Fact]
    public async void ShouldCallContentfulIfCacheIsEmptyAsync()
    {
        // Arrange
        _distributedCacheWrapper
            .Setup(o => o.GetString(It.Is<string>(s => s.Equals("test-key")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        // Act
        string valueFromCall = await _cacheWrapper.GetFromCacheOrDirectlyAsync("test-key", testFallbackMethodAsync);

        // Assert
        valueFromCall.Should().Be("Contentful Data");
        LogTesting.Assert(_logger, LogLevel.Information, "Key 'test-key' not found in cache of type: System.String");
    }

    [Fact]
    public async void ShouldNotCallContentfulIfCacheIsFullAsync()
    {
        // Arrange
        _distributedCacheWrapper
            .Setup(o => o.GetString(It.Is<string>(s => s.Equals("test-key")), It.IsAny<CancellationToken>()))
            .ReturnsAsync("\"Cache Data\"");

        // Act
        string valueFromCall = await _cacheWrapper.GetFromCacheOrDirectlyAsync("test-key", testFallbackMethodAsync);

        // Assert
        valueFromCall.Should().Be("Cache Data");
    }

    public async Task<string> testFallbackMethodAsync() => await Task.FromResult("Contentful Data");
}