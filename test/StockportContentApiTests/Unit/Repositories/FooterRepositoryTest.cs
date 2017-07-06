using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.Factories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class FooterRepositoryTest
    {
        private readonly ContentfulConfig _config;
        private readonly Mock<IContentfulClient> _client;
        private readonly FooterRepository _repository;

        public FooterRepositoryTest()
        {
            _config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            var contentfulClientManager = new Mock<IContentfulClientManager>();

            _client = new Mock<IContentfulClient>();

            contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);

            _repository = new FooterRepository(_config, contentfulClientManager.Object);
        }

        [Fact]
        public void ShouldReturnAFooter()
        {
            var builderFooter = new Footer("Title", "a-slug", "Copyright", new List<SubItem>(),
                new List<SocialMediaLink>());

            var mockFooter = new Footer("test", "test", "test", null, null);

            var builder = new QueryBuilder<Footer>().ContentTypeIs("footer").Include(1);
            var collection = new ContentfulCollection<Footer>();
            collection.Items = new List<Footer> { mockFooter };

            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<Footer>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);


            var footer = AsyncTestHelper.Resolve(_repository.GetFooter());

            footer.Get<Footer>().Should().Be(mockFooter);
            footer.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void ShouldReturn404IfNoEntryExists()
        {
            var builderFooter = new Footer("Title", "a-slug", "Copyright", new List<SubItem>(),
                new List<SocialMediaLink>());

            var mockFooter = new Footer("test", "test", "test", null, null);

            var builder = new QueryBuilder<Footer>().ContentTypeIs("footer").Include(1);
            var collection = new ContentfulCollection<Footer>();
            collection.Items = new List<Footer>();

            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<Footer>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var footer = AsyncTestHelper.Resolve(_repository.GetFooter());

            footer.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
