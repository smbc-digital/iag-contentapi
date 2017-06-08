using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;
using StockportContentApi.Client;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class GroupCategoryRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly GroupCategoryRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _contentfulGroupCategoryFactory;

        public GroupCategoryRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _httpClient = new Mock<IHttpClient>();           

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            _contentfulGroupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            _repository = new GroupCategoryRepository(config, _contentfulGroupCategoryFactory.Object, contentfulClientManager.Object);
        }

        [Fact]
        public void ItGetsGroupCategories()
        {
            // Arrange
            const string slug = "unit-test-GroupCategory";

            var rawGroupCategory = new ContentfulGroupCategoryBuilder().Slug(slug).Build();

            var builder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory");
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulGroupCategory> { rawGroupCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupCategories());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ShouldReturnNotFoundIfNoGroupCategoryFound()
        {
            var builder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory");
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulGroupCategory>());

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupCategories());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
