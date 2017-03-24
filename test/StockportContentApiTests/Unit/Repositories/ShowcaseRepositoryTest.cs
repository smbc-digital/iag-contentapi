using System.Collections.Generic;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;
using StockportContentApi.Client;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Builders;
using File = System.IO.File;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class ShowcaseRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly ShowcaseRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly Mock<IContentfulFactory<ContentfulTopic, Topic>> _topicFactory;

        public ShowcaseRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _httpClient = new Mock<IHttpClient>();
            _topicFactory = new Mock<IContentfulFactory<ContentfulTopic, Topic>>();

            var contentfulFactory = new ShowcaseContentfulFactory(
                _topicFactory.Object);

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            _repository = new ShowcaseRepository(config, contentfulFactory, contentfulClientManager.Object);
        }

        [Fact]
        public void ItGetsShowcase()
        {
            // Arrange
            const string slug = "unit-test-showcase";

            var rawShowcase = new ContentfulShowcaseBuilder().Slug(slug).Build();

            var builder = new QueryBuilder<Entry<ContentfulShowcase>>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulShowcase> { rawShowcase });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetShowcases(slug));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
