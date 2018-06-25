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
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Unit.Repositories
{
    public class FooterRepositoryTest
    {
        private readonly ContentfulConfig _config;
        private readonly Mock<IContentfulClient> _client;
        private readonly FooterRepository _repository;
        private readonly Mock<IContentfulFactory<ContentfulFooter, Footer>> _contentfulFactory;

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
            _contentfulFactory = new Mock<IContentfulFactory<ContentfulFooter, Footer>>();

            contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);

            _repository = new FooterRepository(_config, contentfulClientManager.Object, _contentfulFactory.Object);
        }

        [Fact]
        public void ShouldReturnAFooter()
        {
           var mockFooter = new Footer("Title", "a-slug", "Copyright", new List<SubItem>(), new List<SocialMediaLink>());

            var footerCollection = new ContentfulCollection<ContentfulFooter>();
            footerCollection.Items = new List<ContentfulFooter>
                {
                   new ContentfulFooterBuilder().Build()
                };

            _client.Setup(o => o.GetEntries(
                                It.Is<QueryBuilder<ContentfulFooter>>(q => q.Build() == new QueryBuilder<ContentfulFooter>().ContentTypeIs("footer").Include(1).Build()),
                                It.IsAny<CancellationToken>())).ReturnsAsync(footerCollection);

            _contentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulFooter>()))
                .Returns(new Footer("Title", "a-slug", "Copyright", new List<SubItem>(), 
                    new List<SocialMediaLink>()));
            var footer = AsyncTestHelper.Resolve(_repository.GetFooter());
            footer.Get<Footer>().Title.Should().Be(mockFooter.Title);
            footer.Get<Footer>().Slug.Should().Be(mockFooter.Slug);
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

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<Footer>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var footer = AsyncTestHelper.Resolve(_repository.GetFooter());

            footer.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
