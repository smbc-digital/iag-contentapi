using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Factories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace StockportContentApiTests.Unit.Repositories
{
    public class GroupRepositoryTest
    {
        private readonly GroupRepository _repository;
        private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
        private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
        private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _groupCategoryFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>> _groupHomepageContentfulFactory;
        private readonly Mock<IContentfulClient> _client;
        private readonly Mock<ITimeProvider> _timeProvider;
        private readonly EventRepository _eventRepository;
        private readonly Mock<IHttpClient> _httpClient;
        private readonly Mock<ICache> _cacheWrapper;
        private readonly Mock<ILogger<EventRepository>> _logger;
        private readonly Mock<IConfiguration> _configuration;

        public GroupRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
            _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
            _eventHomepageFactory = new Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>();

            _timeProvider = new Mock<ITimeProvider>();
            _httpClient = new Mock<IHttpClient>();
            _groupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
            _groupHomepageContentfulFactory = new Mock<IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>>();
            _logger = new Mock<ILogger<EventRepository>>();

            _cacheWrapper = new Mock<ICache>();
            _configuration = new Mock<IConfiguration>();
            _configuration.Setup(_ => _["redisExpiryTimes:Groups"]).Returns("60");
            _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");
            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

            _eventRepository = new EventRepository(config, contentfulClientManager.Object, _timeProvider.Object, _eventFactory.Object, _eventHomepageFactory.Object, _cacheWrapper.Object, _logger.Object, _configuration.Object);
            _repository = new GroupRepository(config, contentfulClientManager.Object, _timeProvider.Object, _groupFactory.Object, _groupCategoryFactory.Object, _groupHomepageContentfulFactory.Object, _eventRepository, _cacheWrapper.Object, _configuration.Object);
        }

        [Fact]
        public void GetsGroupForGroupSlug()
        {
            const string slug = "group_slug";
            var contentfulGroup = new ContentfulGroupBuilder().Slug(slug).Build();
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = new List<ContentfulGroup> { contentfulGroup };

            var group = new GroupBuilder().Build();
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<ContentfulEvent>());

            _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, false));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseGroup = response.Get<Group>();
            responseGroup.ShouldBeEquivalentTo(group);
        }

        [Fact]
        public void GetsGroupWhenHiddenDatesAreInThePast()
        {
            // Arrange
            const string slug = "group_slug";
            var contentfulGroup = new ContentfulGroupBuilder().Slug(slug).DateHiddenFrom(DateTime.Now.AddDays(-3)).DateHiddenTo(DateTime.Now.AddDays(-1)).Build();
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = new List<ContentfulGroup> { contentfulGroup };

            var group = new GroupBuilder().Slug("group_slug").DateHiddenFrom(DateTime.Now.AddDays(-3)).DateHiddenTo(DateTime.Now.AddDays(-1)).Build();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<ContentfulEvent>());

            _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseGroup = response.Get<Group>();
            responseGroup.ShouldBeEquivalentTo(group);
        }

        [Fact]
        public void GetsGroupWhenHiddenDatesAreInTheFuture()
        {
            // Arrange
            const string slug = "group_slug";
            var contentfulGroup = new ContentfulGroupBuilder().Slug(slug).DateHiddenFrom(DateTime.Now.AddDays(1)).DateHiddenTo(DateTime.Now.AddDays(3)).Build();
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = new List<ContentfulGroup> { contentfulGroup };

            var group = new GroupBuilder().Slug("group_slug").DateHiddenFrom(DateTime.Now.AddDays(1)).DateHiddenTo(DateTime.Now.AddDays(3)).Build();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<ContentfulEvent>());

            _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseGroup = response.Get<Group>();
            responseGroup.ShouldBeEquivalentTo(group);
        }

        [Fact]
        public void DoesntGetGroupWhenHiddenDatesAreNow()
        {
            // Arrange
            const string slug = "group_slug";
            var contentfulGroup = new ContentfulGroupBuilder().Slug(slug).DateHiddenFrom(DateTime.Now.AddDays(-3)).DateHiddenTo(DateTime.Now.AddDays(3)).Build();
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = new List<ContentfulGroup> { contentfulGroup };

            var group = new GroupBuilder().Slug("group_slug").DateHiddenFrom(DateTime.Now.AddDays(-3)).DateHiddenTo(DateTime.Now.AddDays(3)).Build();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<ContentfulEvent>());

            _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseGroup = response.Get<Group>();
            responseGroup.ShouldBeEquivalentTo(null);
        }

        [Fact]
        public void DoesntGetGroupWhenHiddenDateFromIsInThePastAndHiddenDateToIsNull()
        {
            // Arrange
            const string slug = "group_slug";
            var contentfulGroup = new ContentfulGroupBuilder().Slug(slug).DateHiddenFrom(DateTime.Now.AddDays(-3)).Build();
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = new List<ContentfulGroup> { contentfulGroup };

            var group = new GroupBuilder().Slug("group_slug").DateHiddenFrom(DateTime.Now.AddDays(-3)).Build();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<ContentfulEvent>());

            _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseGroup = response.Get<Group>();
            responseGroup.ShouldBeEquivalentTo(null);
        }

        [Fact]
        public void DoesntGetGroupWhenHiddenDateToIsInTheFutureAndHiddenDateFromIsNull()
        {
            // Arrange
            const string slug = "group_slug";
            var contentfulGroup = new ContentfulGroupBuilder().Slug(slug).DateHiddenTo(DateTime.Now.AddDays(3)).Build();
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = new List<ContentfulGroup> { contentfulGroup };

            var group = new GroupBuilder().Slug("group_slug").DateHiddenTo(DateTime.Now.AddDays(3)).Build();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<ContentfulEvent>());

            _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseGroup = response.Get<Group>();
            responseGroup.ShouldBeEquivalentTo(null);
        }

        [Fact]
        public void Return404WhenGroupWhenItemsDontExist()
        {
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = new List<ContentfulGroup>();

            const string slug = "not-found";
            _client.Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulGroup>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, false));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be($"No group found for '{slug}'");
        }

        [Fact]
        public void ShouldGetAllGroupsWithSpecifiedCategory()
        {
            // Arrange
            var testCategorySlug = "test-category-slug";
            var listOfContentfulGroups = SetupMockFactoriesAndGetContentfulGroupsForCollection(testCategorySlug);
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = listOfContentfulGroups;
            var listOfGroupCategories = new List<GroupCategory> { new GroupCategory("name", testCategorySlug, "icon", "image-url.jpg") };

            // Act
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).FieldEquals("fields.mapPosition[near]", Defaults.Groups.StockportLatitude + "," + Defaults.Groups.StockportLongitude + ",10").Build();
            _client.Setup(o => o.GetEntries<ContentfulGroup>(It.Is<string>(q => q.Contains(builder)),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var categoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "group-categories"), It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(listOfGroupCategories);

            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults(testCategorySlug, Defaults.Groups.StockportLatitude, Defaults.Groups.StockportLongitude, "name a-z", "Stockport", string.Empty, string.Empty, string.Empty, string.Empty));
            var filteredGroupResults = response.Get<GroupResults>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            filteredGroupResults.Groups.Count.Should().Be(1);
            filteredGroupResults.Groups[0].Slug.Should().Be("slug-with-categories");
        }

        [Fact]
        public void ShouldReturnEmptyListIfNoGroupsMatchTheCategory()
        {
            // Arrange
            var testCategorySlug = "test-category-slug";
            var listOfContentfulGroups = SetupMockFactoriesAndGetContentfulGroupsForCollection(testCategorySlug);
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = listOfContentfulGroups;
            var listOfContentfulGroupCategories = new List<ContentfulGroupCategory> { new ContentfulGroupCategory() { Slug = "slug-that-matches-no-groups" } };
            
            var listOfGroupCategories = new List<GroupCategory> { new GroupCategory("name", "slug-that-matches-no-groups", "icon", "image-url.jpg") };

            // Act
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).FieldEquals("fields.mapPosition[near]", Defaults.Groups.StockportLatitude + "," + Defaults.Groups.StockportLongitude + ",10").Build();
            _client.Setup(o => o.GetEntries<ContentfulGroup>(It.Is<string>(q => q.Contains(builder)),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "group-categories"), It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(listOfGroupCategories);

            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults("slug-that-matches-no-groups", Defaults.Groups.StockportLatitude, Defaults.Groups.StockportLongitude, "name a-z", "Stockport", string.Empty, string.Empty, string.Empty, string.Empty));
            var filteredGroupResults = response.Get<GroupResults>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            filteredGroupResults.Groups.Count.Should().Be(0);
        }

        [Fact]
        public void ShouldReturnAListOfAListOfCategories()
        {
            // Arrange
            const string slug = "unit-test-GroupCategory";
            var testCategorySlug = "test-category-slug";
            var listOfContentfulGroups = SetupMockFactoriesAndGetContentfulGroupsForCollection(testCategorySlug);
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = listOfContentfulGroups;

            var rawContentfulGroupCategory = new ContentfulGroupCategoryBuilder().Slug(slug).Build();
            var rawGroupCategory = new GroupCategory("name", slug, "icon", "imageUrl");

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).FieldEquals("fields.mapPosition[near]", Defaults.Groups.StockportLatitude + "," + Defaults.Groups.StockportLongitude + ",10").Build();
            _client.Setup(o => o.GetEntries<ContentfulGroup>(It.Is<string>(q => q.Contains(builder)),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "group-categories"), It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<GroupCategory>() { rawGroupCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults(slug, Defaults.Groups.StockportLatitude, Defaults.Groups.StockportLongitude, "name a-z", "Stockport", string.Empty, string.Empty, string.Empty, string.Empty));
            var filteredGroupResults = response.Get<GroupResults>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            filteredGroupResults.Categories.Count.Should().Be(1);
        }

        [Fact]
        public void ShouldReturn404IfSpecifiedCategoryDoesntExist()
        {
            // Arrange
            const string slug = "unit-test-GroupCategory";
            var testCategorySlug = "test-category-slug";
            var listOfContentfulGroups = SetupMockFactoriesAndGetContentfulGroupsForCollection(testCategorySlug);
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = listOfContentfulGroups;

            var rawContentfulGroupCategory = new ContentfulGroupCategoryBuilder().Slug(slug).Build();
            var rawGroupCategory = new GroupCategory("name", slug, "icon", "imageUrl");

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).FieldEquals("fields.mapPosition[near]", Defaults.Groups.StockportLatitude + "," + Defaults.Groups.StockportLongitude + ",10").Build();
            _client.Setup(o => o.GetEntries<ContentfulGroup>(It.Is<string>(q => q.Contains(builder)),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "group-categories"), It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<GroupCategory>() { rawGroupCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults("fake-category-slug", Defaults.Groups.StockportLatitude, Defaults.Groups.StockportLongitude, "name a-z", "Stockport", string.Empty, string.Empty, string.Empty, string.Empty));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void ShouldReturnALocationWhenSelected()
        {
            // Arrange
            const string slug = "unit-test-GroupCategory";
            MapPosition location = new MapPosition() { Lat = 1, Lon = 1 };
            var contentfulGroupWithlocation = new ContentfulGroupBuilder().Slug(slug).MapPosition(location).Build();
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = new List<ContentfulGroup> { contentfulGroupWithlocation };

            var groupWithLocation = new GroupBuilder().MapPosition(location).Slug(slug).Build();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _groupFactory.Setup(o => o.ToModel(contentfulGroupWithlocation)).Returns(groupWithLocation);
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<ContentfulEvent>());

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, false));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseProfile = response.Get<Group>();
            responseProfile.MapPosition.ShouldBeEquivalentTo(location);
        }

        [Fact]
        public void ShouldReturnVolunteeringWhenSelected()
        {
            // Arrange
            const string slug = "unit-test-GroupCategory";
            MapPosition location = new MapPosition() { Lat = 1, Lon = 1 };
            var contentfulGroupWithlocation = new ContentfulGroupBuilder().Slug(slug).MapPosition(location).Build();
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = new List<ContentfulGroup> { contentfulGroupWithlocation };

            bool volunteering = true;
            var groupWithLocation = new GroupBuilder().Volunteering(volunteering).Slug(slug).Build();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<ContentfulEvent>());
            _groupFactory.Setup(o => o.ToModel(contentfulGroupWithlocation)).Returns(groupWithLocation);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, false));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseProfile = response.Get<Group>();
            responseProfile.Volunteering.Should().Be(volunteering);
        }

        [Fact]
        public void ShouldOrderByAToZAsSpecified()
        {
            // Arrange
            var groupCategory = new GroupCategory("name", "slug", "icon", "image");
            var location = new MapPosition() { Lat = 5, Lon = -2 };

            var contentfulGroupFirst = new ContentfulGroupBuilder().Slug("slug1").Build();
            var contentfulGroupSecond = new ContentfulGroupBuilder().Slug("slug2").Build();
            var contentfulGroupThird = new ContentfulGroupBuilder().Slug("slug3").Build();
            
            var collection = new ContentfulCollection<ContentfulGroup>
            {
                Items = new List<ContentfulGroup> {contentfulGroupFirst, contentfulGroupSecond, contentfulGroupThird}
            };

            var groupfirst = new GroupBuilder().Name("aGroup").Slug("slug1").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();
            var groupsecond = new GroupBuilder().Name("bGroup").Slug("slug2").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();
            var groupthird = new GroupBuilder().Name("cGroup").Slug("slug3").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "group-categories"), It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<GroupCategory>() { groupCategory });

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).FieldEquals("fields.mapPosition[near]", Defaults.Groups.StockportLatitude + "," + Defaults.Groups.StockportLongitude + ",10").Build();
            _client.Setup(o => o.GetEntries< ContentfulGroup>(It.Is<string>(q => q.Contains(builder)),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _groupFactory.Setup(o => o.ToModel(contentfulGroupFirst)).Returns(groupfirst);
            _groupFactory.Setup(o => o.ToModel(contentfulGroupSecond)).Returns(groupsecond);
            _groupFactory.Setup(o => o.ToModel(contentfulGroupThird)).Returns(groupthird);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults("slug", Defaults.Groups.StockportLatitude, Defaults.Groups.StockportLongitude, "name a-z", "Stockport", string.Empty, string.Empty, string.Empty, string.Empty));
            var filteredGroupResults = response.Get<GroupResults>();

            // Assert
            filteredGroupResults.Groups.First().Should().Be(groupfirst);
            filteredGroupResults.Groups[1].Should().Be(groupsecond);
            filteredGroupResults.Groups[2].Should().Be(groupthird);
        }

        [Fact]
        public void ShouldOrderByZToAAsSpecified()
        {
            // Arrange
            var groupCategory = new GroupCategory("name", "slug", "icon", "image");
            var location = new MapPosition() { Lat = 5, Lon = -2 };
            var contentfulGroupFirst = new ContentfulGroupBuilder().Slug("slug1").Build();
            var contentfulGroupSecond = new ContentfulGroupBuilder().Slug("slug2").Build();
            var contentfulGroupThird = new ContentfulGroupBuilder().Slug("slug3").Build();
            var collection = new ContentfulCollection<ContentfulGroup>
            {
                Items = new List<ContentfulGroup> { contentfulGroupFirst, contentfulGroupSecond, contentfulGroupThird }
            };

            var groupfirst = new GroupBuilder().Name("aGroup").Slug("slug1").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();
            var groupsecond = new GroupBuilder().Name("bGroup").Slug("slug2").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();
            var groupthird = new GroupBuilder().Name("cGroup").Slug("slug3").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).FieldEquals("fields.mapPosition[near]", Defaults.Groups.StockportLatitude + "," + Defaults.Groups.StockportLongitude + ",10").Build();
            _client.Setup(o => o.GetEntries<ContentfulGroup>(It.Is<string>(q => q.Contains(builder)),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _groupFactory.Setup(o => o.ToModel(contentfulGroupFirst)).Returns(groupfirst);
            _groupFactory.Setup(o => o.ToModel(contentfulGroupSecond)).Returns(groupsecond);
            _groupFactory.Setup(o => o.ToModel(contentfulGroupThird)).Returns(groupthird);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "group-categories"), It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(new List<GroupCategory>() { groupCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults("slug", Defaults.Groups.StockportLatitude, Defaults.Groups.StockportLongitude, "name z-a", "Stockport", string.Empty, string.Empty, string.Empty, string.Empty));
            var filteredGroupResults = response.Get<GroupResults>();

            // Assert
            filteredGroupResults.Groups.First().Should().Be(groupthird);
            filteredGroupResults.Groups[1].Should().Be(groupsecond);
            filteredGroupResults.Groups[2].Should().Be(groupfirst);
        }

        

        [Fact]
        public void ShouldReturnListOfGroupsWhereAdministratorMatchesEmailAddress()
        {
            // Arrange
            var emailAddress = "test@test.com";
            var contentfulGroupsReturned = new List<ContentfulGroup>();
            var correctContentfulGroup = new ContentfulGroupBuilder().GroupAdministrators(new GroupAdministrators() { Items = new List<GroupAdministratorItems>() { new GroupAdministratorItems() {Email = emailAddress, Permission = "A"} } }).Build();
            contentfulGroupsReturned.Add(new ContentfulGroupBuilder().GroupAdministrators(new GroupAdministrators() { Items = new List<GroupAdministratorItems> { new GroupAdministratorItems() { Email = "bill@yahoo.com", Permission = "E" } } }).Build());
            contentfulGroupsReturned.Add(correctContentfulGroup);
            contentfulGroupsReturned.Add(new ContentfulGroupBuilder().GroupAdministrators(new GroupAdministrators() { Items = new List<GroupAdministratorItems> { new GroupAdministratorItems() { Email = "fred@msn.com", Permission = "A"} } }).Build());
            contentfulGroupsReturned.Add(new ContentfulGroupBuilder().GroupAdministrators(new GroupAdministrators() { Items = new List<GroupAdministratorItems> { new GroupAdministratorItems() { Email = "jerry@gmail.com", Permission = "A" } } }).Build());
            var collection = new ContentfulCollection<ContentfulGroup>();
            collection.Items = contentfulGroupsReturned ;

            var groupReturned = new GroupBuilder().GroupAdministrators(new GroupAdministrators() { Items = new List<GroupAdministratorItems>() { new GroupAdministratorItems() { Email = emailAddress, Permission = "A" } } }).Build();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldExists("fields.groupAdministrators")
                            .Include(1)
                            .Limit(ContentfulQueryValues.LIMIT_MAX);
            _client.Setup(
                o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                    It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _groupFactory.Setup(o => o.ToModel(correctContentfulGroup)).Returns(groupReturned);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetAdministratorsGroups(emailAddress));
            var filteredGroups = response.Get<List<Group>>();

            // Assert
            filteredGroups.Count().Should().Be(1);

        }

        [Fact]
        public void ShouldReturnContenfulGroupHomepage()
        {
            var contenfulHomepage = new ContentfulGroupHomepageBuilder().Build();
            var collection = new ContentfulCollection<ContentfulGroupHomepage>();
            collection.Items = new List<ContentfulGroupHomepage> { contenfulHomepage };

            var groupHomepage = new GroupHomepage("title", "slug", "image-url.jpg", string.Empty, null, null, null, null, string.Empty, string.Empty, string.Empty, string.Empty);

            var builder = new QueryBuilder<ContentfulGroupHomepage>().ContentTypeIs("groupHomepage").Include(1);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupHomepage>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);
            
            _groupHomepageContentfulFactory.Setup(o => o.ToModel(contenfulHomepage)).Returns(groupHomepage);

            var response = AsyncTestHelper.Resolve(_repository.GetGroupHomepage());
            var homepage = response.Get<GroupHomepage>();

            homepage.BackgroundImage.Should().Be(contenfulHomepage.BackgroundImage.File.Url);
            homepage.Title.Should().Be(contenfulHomepage.Title);
            homepage.Slug.Should().Be(contenfulHomepage.Slug);
        }

        private List<ContentfulGroup> SetupMockFactoriesAndGetContentfulGroupsForCollection(string testCategorySlug)
        {
            var contentfulGroupCategory = new ContentfulGroupCategoryBuilder().Slug(testCategorySlug).Build();
            var contentfulGroupWithCategory = new ContentfulGroupBuilder().Slug("slug-with-categories").CategoriesReference(
                new List<ContentfulGroupCategory> { contentfulGroupCategory }).Build();
            var contentfulGroupWithoutCategory = new ContentfulGroupBuilder().Slug("slug-without-categories").Build();

            var groupCategory = new GroupCategory("name", testCategorySlug, "icon", "imagueUrl");
            var groupWithCategory = new GroupBuilder().Slug("slug-with-categories").CategoriesReference(
                    new List<GroupCategory>()
                    {
                        groupCategory
                    })
                .Build();

            var groupWithoutCategory = new GroupBuilder().Slug("slug-without-categories").Build();

            _groupFactory.Setup(o => o.ToModel(contentfulGroupWithCategory)).Returns(groupWithCategory);
            _groupFactory.Setup(o => o.ToModel(contentfulGroupWithoutCategory)).Returns(groupWithoutCategory);

            return new List<ContentfulGroup>
            {
                contentfulGroupWithCategory,
                contentfulGroupWithoutCategory
            };
        }
    }
}