using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Contentful.Core;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class SmartResultRepositoryTests
    {
        private readonly Mock<IContentfulClientManager> _clientManager = new Mock<IContentfulClientManager>();
        private readonly Mock<IContentfulClient> _client = new Mock<IContentfulClient>();
        private readonly ISmartResultRepository _repository;
        private readonly Mock<ILogger<SmartResultRepository>> _logger = new Mock<ILogger<SmartResultRepository>>();
        private readonly Mock<IConfiguration> _configuration;

        public SmartResultRepositoryTests()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _configuration = new Mock<IConfiguration>();
            _clientManager.Setup(_ => _.GetClient(It.IsAny<ContentfulConfig>())).Returns(_client.Object);
            _repository = new SmartResultRepository(config, _logger.Object, _clientManager.Object);

        }

        [Fact]
        public void ShouldCallContentfulClient()
        {
            // Arrange
            const string slug = "a-slug";
            var builder = new QueryBuilder<ContentfulSmartResult>().ContentTypeIs("smartResult").FieldEquals("fields.slug", slug).Include(3);
            _client.Setup(_ => _.GetEntries(
                It.Is<QueryBuilder<ContentfulSmartResult>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<ContentfulCollection<ContentfulSmartResult>>());

            // Act
            _repository.Get(slug);

            // Assert
            _client.Verify(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulSmartResult>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()),Times.Once);
        }

        [Fact]
        public async void GetShouldReturnAnEntry()
        {
            // Arrange
            const string slug = "a-slug";
            var entry = new ContentfulSmartResult
            {
                Body = "",
                Slug = slug
            };
            var entries = new ContentfulCollection<ContentfulSmartResult>
            {
                Items = new List<ContentfulSmartResult>
                {
                    entry
                }
            };

            var builder = new QueryBuilder<ContentfulSmartResult>().ContentTypeIs("smartResult").FieldEquals("fields.slug", slug).Include(3);
            _client.Setup(_ => _.GetEntries(
                It.Is<QueryBuilder<ContentfulSmartResult>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(entries);

            // Act
            var result = await _repository.Get(slug);

            // Assert
            _client.Verify(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulSmartResult>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()), Times.Once);
            result.Should().Be(entry);
        }

        [Fact]
        public async void GetShouldLogAnError_WhenAnExceptionIsThrown()
        {
            // Arrange
            var slug = "a-slug";
            var builder = new QueryBuilder<ContentfulSmartResult>().ContentTypeIs("smartResult").FieldEquals("fields.slug", slug).Include(3);
            _client.Setup(_ => _.GetEntries(
                It.Is<QueryBuilder<ContentfulSmartResult>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ThrowsAsync(new ContentfulException(200,"error"));

            // Act
            await _repository.Get(slug);

            // Assert
            LogTesting.Assert(_logger, LogLevel.Warning, $"There was a problem with getting SmartResult with slug: {slug} from contentful");

        }
    }
}
