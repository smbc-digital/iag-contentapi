using FluentAssertions;
using Moq;
using StockportContentApi.Utils;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StockportContentApiTests.Unit.Utils
{
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
            _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"))).ReturnsAsync("");

            // Act
            var valueFromCall = _cacheWrapper.GetFromCacheOrDirectly("test-key", testFallbackMethod);

            // Assert
            valueFromCall.Should().Be("Contentful Data");
        }

        [Fact]
        public void ShouldNotCallContentfulIfCacheIsFull()
        {
            // Arrange
            _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"))).ReturnsAsync("\"Cache Data\"");

            // Act
            var valueFromCall = _cacheWrapper.GetFromCacheOrDirectly("test-key", testFallbackMethod);

            // Assert
            valueFromCall.Should().Be("Cache Data");
        }

        private string testFallbackMethod()
        {
            return "Contentful Data";
        }

        [Fact]
        public async void ShouldCallContentfulIfCacheIsEmptyAsync()
        {
            // Arrange
            _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"))).ReturnsAsync("");

            // Act
            var valueFromCall = await _cacheWrapper.GetFromCacheOrDirectlyAsync("test-key", testFallbackMethodAsync);

            // Assert
            valueFromCall.Should().Be("Contentful Data");
            LogTesting.Assert(_logger, LogLevel.Information, "Key not found in cache:test-key of type:System.String");
        }

        [Fact]
        public async void ShouldNotCallContentfulIfCacheIsFullAsync()
        {
            // Arrange
            _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"))).ReturnsAsync("\"Cache Data\"");

            // Act
            var valueFromCall = await _cacheWrapper.GetFromCacheOrDirectlyAsync("test-key", testFallbackMethodAsync);

            // Assert
            valueFromCall.Should().Be("Cache Data");
        }

        private async Task<string> testFallbackMethodAsync()
        {
            return "Contentful Data";
        }
    }
}
