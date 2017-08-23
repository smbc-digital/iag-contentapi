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
using System.Threading.Tasks;

namespace StockportContentApiTests.Unit.Repositories
{
    public class EventCategoryRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly EventCategoryRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>> _contentfulEventCategoryFactory;
        private readonly Mock<ICache> _cacheWrapper;

        public EventCategoryRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _httpClient = new Mock<IHttpClient>();           

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            _contentfulEventCategoryFactory = new Mock<IContentfulFactory<ContentfulEventCategory, EventCategory>>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
            _cacheWrapper = new Mock<ICache>();

            _repository = new EventCategoryRepository(config, _contentfulEventCategoryFactory.Object, contentfulClientManager.Object, _cacheWrapper.Object);
        }

        [Fact]
        public void ItGetsEventCategories()
        {
            // Arrange
            var rawEventCategory = new EventCategory("name", "slug", "icon");
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-categories-content-type"), It.IsAny<Func<Task<List<EventCategory>>>>())).ReturnsAsync(new List<EventCategory> { rawEventCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetEventCategories());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ShouldReturnNotFoundIfNoEventCategoryFound()
        {
            var collection = new ContentfulCollection<ContentfulEventCategory>();
            collection.Items = new List<ContentfulEventCategory>();

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-categories-content-type"), It.IsAny<Func<Task<List<EventCategory>>>>())).ReturnsAsync(new List<EventCategory>());

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetEventCategories());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
