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
    public class ProfileRepositoryTest
    {
        private readonly ProfileRepository _repository;
        private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;
        private readonly Mock<IContentfulClient> _client;

        public ProfileRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

            _repository = new ProfileRepository(config, contentfulClientManager.Object, _profileFactory.Object);
        }

        [Fact]
        public void GetsProfileForProfileSlug()
        {
            const string slug = "a-slug";
            var contentfulTopic = new ContentfulProfileBuilder().Slug(slug).Build();
            var profile = new Profile("type", "title", "slug", "subtitle",
                "teaser", "image", "body", "icon", "backgroundImage",
                new List<Crumb> { new Crumb("title", "slug", "type") });
            var builder = new QueryBuilder().ContentTypeIs("profile").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntriesAsync<ContentfulProfile>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulProfile> { contentfulTopic });
            _profileFactory.Setup(o => o.ToModel(contentfulTopic)).Returns(profile);

            var response = AsyncTestHelper.Resolve(_repository.GetProfile(slug));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseProfile = response.Get<Profile>();
            responseProfile.ShouldBeEquivalentTo(profile);

        }

        [Fact]
        public void Return404WhenProfileWhenItemsDontExist()
        {
            const string slug = "not-found";
            var builder = new QueryBuilder().ContentTypeIs("profile").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntriesAsync<ContentfulProfile>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulProfile>());

            var response = AsyncTestHelper.Resolve(_repository.GetProfile(slug));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be($"No profile found for '{slug}'");
        }
    }
}