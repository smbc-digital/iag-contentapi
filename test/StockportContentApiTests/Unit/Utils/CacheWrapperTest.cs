using System;
using FluentAssertions;
using Moq;
using StockportContentApi.Utils;
using Xunit;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace StockportContentApiTests.Unit.Utils
{
    public class CacheTest
    {
        private readonly Cache _cacheWrapper;
        private readonly Mock<IDistributedCacheWrapper> _distributedCacheWrapper;

        public CacheTest()
        {
            _distributedCacheWrapper = new Mock<IDistributedCacheWrapper>();
            _cacheWrapper = new Cache(_distributedCacheWrapper.Object);
            

        }

        [Fact]
        public void ShouldCallContentfulIfCacheIsEmpty()
        {
            // Arrange
            _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"))).Returns("");

            // Act
            var valueFromCall = _cacheWrapper.GetFromCacheOrDirectly("test-key", testFallbackMethod);

            // Assert
            valueFromCall.Should().Be("Contentful Data");
        }

        [Fact]
        public void ShouldNotCallContentfulIfCacheIsFull()
        {
            // Arrange
            _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"))).Returns("\"Cache Data\"");

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
            _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"))).Returns("");

            // Act
            var valueFromCall = await _cacheWrapper.GetFromCacheOrDirectlyAsync("test-key", testFallbackMethodAsync);

            // Assert
            valueFromCall.Should().Be("Contentful Data");
        }

        [Fact]
        public async void ShouldNotCallContentfulIfCacheIsFullAsync()
        {
            // Arrange
            _distributedCacheWrapper.Setup(o => o.GetString(It.Is<string>(s => s == "test-key"))).Returns("\"Cache Data\"");

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
