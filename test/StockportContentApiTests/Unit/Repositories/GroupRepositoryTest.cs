using System.Collections.Generic;
using System.Net;
using System.Threading;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class GroupRepositoryTest
    {
        private readonly GroupRepository _repository;
        private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
        private readonly Mock<IContentfulClient> _client;

        public GroupRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

            _repository = new GroupRepository(config, contentfulClientManager.Object, _groupFactory.Object);
        }

        [Fact]
        public void GetsGroupForGroupSlug()
        {
            const string slug = "group_slug";
            var contentfulGroup = new ContentfulGroupBuilder().Slug(slug).Build();
            var group = new Group("name", "group_slug", "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl");
            var builder = new QueryBuilder().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntriesAsync<ContentfulGroup>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroup> { contentfulGroup });
            _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseProfile = response.Get<Group>();
            responseProfile.ShouldBeEquivalentTo(group);
        }

        [Fact]
        public void Return404WhenGroupWhenItemsDontExist()
        {
            const string slug = "not-found";
            var builder = new QueryBuilder().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntriesAsync<ContentfulGroup>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroup>());

            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be($"No group found for '{slug}'");
        }
    }
}