using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class PrivacyNoticeRepositoryTest
    {
        private readonly PrivacyNoticeRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>> _contentfulFactory;

        public PrivacyNoticeRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _contentfulFactory = new Mock<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>>();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();

            contentfulClientManager
                .Setup(o => o.GetClient(config))   
                .Returns(_contentfulClient.Object);

            _repository = new PrivacyNoticeRepository(config, _contentfulFactory.Object, contentfulClientManager.Object);
        }

        [Fact]
        public void GetPrivacyNotice_ShouldCallContentful()
        {
            // Arrange
            const string slug = "test-slug";
            var contentfulPrivacyNotice = new ContentfulPrivacyNotice()
            {
                Slug = slug
            };
            var privacyNotice = new PrivacyNotice()
            {
                Slug = slug
            };
            var contentfulCollection = new ContentfulCollection<ContentfulPrivacyNotice>()
            {
                Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
            };

            _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
            _contentfulFactory.Setup(_ => _.ToModel(contentfulPrivacyNotice)).Returns(privacyNotice);
            // Act
            var result = AsyncTestHelper.Resolve(_repository.GetPrivacyNotice(slug));
            // Assert
            _contentfulClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void GetPrivacyNotice_ShouldReturnAPrivacyNotice()
        {
            // Arrange
            const string slug = "test-slug";
            var contentfulPrivacyNotice = new ContentfulPrivacyNotice()
            {
                Slug = slug
            };
            var privacyNotice = new PrivacyNotice()
            {
                Slug = slug
            };
            var contentfulCollection = new ContentfulCollection<ContentfulPrivacyNotice>()
            {
                Items = new List<ContentfulPrivacyNotice> { contentfulPrivacyNotice }
            };

            _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
            _contentfulFactory.Setup(_ => _.ToModel(contentfulPrivacyNotice)).Returns(privacyNotice);

            // Act
            var result = AsyncTestHelper.Resolve(_repository.GetPrivacyNotice(slug));

            // Assert
            result.Should().BeOfType<PrivacyNotice>();
        }

        [Fact]
        public void GetAllPrivacyNotices_ShouldCallContentful()
        {
            // Arrange
            var contentfulPrivacyNotices = new List<ContentfulPrivacyNotice>()
            {
                new ContentfulPrivacyNotice(),
                new ContentfulPrivacyNotice(),
                new ContentfulPrivacyNotice()
            };
            var privacyNotice = new PrivacyNotice()
            {
                Slug = "slug"
            };
            var contentfulCollection = new ContentfulCollection<ContentfulPrivacyNotice>()
            {
                Items = contentfulPrivacyNotices
            };

            _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
            _contentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulPrivacyNotice>())).Returns(privacyNotice);
            // Act
            var result = AsyncTestHelper.Resolve(_repository.GetAllPrivacyNotices());
            // Assert
            _contentfulClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void GetAllPrivacyNotices_ShouldReturnAListOfPrivacyNotices()
        {
            // Arrange
            var contentfulPrivacyNotices = new List<ContentfulPrivacyNotice>()
            {
                new ContentfulPrivacyNotice(),
                new ContentfulPrivacyNotice(),
                new ContentfulPrivacyNotice()
            };
            var privacyNotice = new PrivacyNotice()
            {
                Slug = "slug"
            };
            var contentfulCollection = new ContentfulCollection<ContentfulPrivacyNotice>()
            {
                Items = contentfulPrivacyNotices
            };

            _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulPrivacyNotice>>(), It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);
            _contentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulPrivacyNotice>())).Returns(privacyNotice);

            // Act
            var result = AsyncTestHelper.Resolve(_repository.GetAllPrivacyNotices());

            // Assert
            result.Should().BeOfType<List<PrivacyNotice>>();
        }
    }
}
