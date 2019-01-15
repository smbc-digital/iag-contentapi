using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
using Contentful.Core.Models;

namespace StockportContentApiTests.Unit.Repositories
{
    public class ProfileRepositoryTest
    {
        private readonly ProfileRepository _repository;
        private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;
        private readonly Mock<IContentfulFactory<ContentfulProfileNew, ProfileNew>> _profileNewFactory;
        private readonly Mock<IContentfulClient> _client;

        public ProfileRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _profileNewFactory = new Mock<IContentfulFactory<ContentfulProfileNew, ProfileNew>>();
            _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

            _repository = new ProfileRepository(config, contentfulClientManager.Object, _profileFactory.Object, _profileNewFactory.Object);
        }

        [Fact]
        public void GetsProfileForProfileSlug()
        {
            const string slug = "a-slug";
            var contentfulTopic = new ContentfulProfileBuilder().Slug(slug).Build();
            var collection = new ContentfulCollection<ContentfulProfile>();
            collection.Items = new List<ContentfulProfile> { contentfulTopic };

            var profile = new Profile("type",
                "title",
                "slug",
                "subtitle",
                "teaser",
                "quote",
                "image",
               "body",
                "icon",
                "backgroundImage",
                new List<Crumb>
                {
                    new Crumb("title", "slug", "type")
                },
                new List<Alert>
                {
                    new Alert("title",
                        "subheading",
                        "body",
                        "severity",
                        DateTime.MinValue,
                        DateTime.MaxValue,
                        "slug")
                });
            var builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", slug).Include(1);


            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulProfile>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);
            _profileFactory.Setup(o => o.ToModel(contentfulTopic)).Returns(profile);

            var response = AsyncTestHelper.Resolve(_repository.GetProfile(slug));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseProfile = response.Get<Profile>();
            responseProfile.Should().BeEquivalentTo(profile);

        }

        [Fact]
        public void Return404WhenProfileWhenItemsDontExist()
        {
            const string slug = "not-found";

            var collection = new ContentfulCollection<ContentfulProfile>();
            collection.Items = new List<ContentfulProfile>();

            var builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var response = AsyncTestHelper.Resolve(_repository.GetProfile(slug));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be($"No profile found for '{slug}'");
        }

        [Fact]
        public async Task GetProfileNew_ShouldGetEntries()
        {
            // Arrange
            _client
                .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfileNew>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContentfulCollection<ContentfulProfileNew>
                {
                    Items = new List<ContentfulProfileNew>()
                });
            // Act
            await _repository.GetProfileNew("fake slug");

            // Assert
            _client.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfileNew>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetProfileNew_ShouldReturnFailureWhenNoEntries()
        {
            // Arrange
            _client
                .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfileNew>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContentfulCollection<ContentfulProfileNew>
                {
                    Items = new List<ContentfulProfileNew>()
                });
            // Act
            var response = await _repository.GetProfileNew("fake slug");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetProfileNew_ShouldReturnSuccessWhenEntriesExist()
        {
            // Arrange
            _client
                .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfileNew>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContentfulCollection<ContentfulProfileNew>
                {
                    Items = new List<ContentfulProfileNew>(){
                        new ContentfulProfileNew(){
                            Slug = "slug"
                        }
                    }
                });
            // Act
            var response = await _repository.GetProfileNew("fake slug");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}