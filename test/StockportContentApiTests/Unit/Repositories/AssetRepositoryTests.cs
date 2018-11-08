using System.Threading;
using Contentful.Core;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApiTests.Unit.Repositories
{
    public class AssetRepositoryTests
    {
        private Mock<IContentfulClientManager> _contentfulClientManager = new Mock<IContentfulClientManager>();
        private Mock<ILogger<AssetRepository>> _logger = new Mock<ILogger<AssetRepository>>();
        private Mock<IContentfulClient> _contentfulClient = new Mock<IContentfulClient>();

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

            var assetRepository = new AssetRepository(new ContentfulConfig("", "", ""), _contentfulClientManager.Object,
                _logger.Object);
            
            var asset = await assetRepository.Get("asset");

            asset.Should().NotBeNull();
        }

        [Fact]
        public async void ShouldReturnNullIfNoAssetIsFoundAndLogWarning()
        {
            _contentfulClient.Setup(o =>
                    o.GetAsset("asset-fail", It.IsAny<QueryBuilder<Asset>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ContentfulException(500, "There was a problem with getting assetid: asset-fail from contentful"));

            var assetRepository = new AssetRepository(new ContentfulConfig("", "", ""), _contentfulClientManager.Object,
                _logger.Object);

            await assetRepository.Get("asset-fail");

            LogTesting.Assert(_logger, LogLevel.Warning, "There was a problem with getting assetId: asset-fail from contentful");
        }
    }
}
