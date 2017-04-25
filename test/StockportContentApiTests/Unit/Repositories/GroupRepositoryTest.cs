﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class GroupRepositoryTest
    {
        private readonly GroupRepository _repository;
        private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
        private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
        private readonly Mock<IContentfulFactory<List<ContentfulGroup>, List<Group>>> _listGroupFactory;
        private readonly Mock<IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>>> _listGroupCategoryFactory;
        private readonly Mock<IContentfulClient> _client;
        private readonly Mock<ITimeProvider> _timeProvider; 

        public GroupRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
            _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
            _timeProvider = new Mock<ITimeProvider>();
            _listGroupFactory = new Mock<IContentfulFactory<List<ContentfulGroup>, List<Group>>>();
            _listGroupCategoryFactory = new Mock<IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>>>();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

            _repository = new GroupRepository(config, contentfulClientManager.Object, _timeProvider.Object, _groupFactory.Object, _listGroupFactory.Object, _listGroupCategoryFactory.Object, _eventFactory.Object);
        }

        [Fact]
        public void GetsGroupForGroupSlug()
        {
            const string slug = "group_slug";
            var contentfulGroup = new ContentfulGroupBuilder().Slug(slug).Build();
            var group = new Group("name", "group_slug", "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, null, false);
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);

            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroup> { contentfulGroup });
            _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseGroup = response.Get<Group>();
            responseGroup.ShouldBeEquivalentTo(group);
        }

        [Fact]
        public void Return404WhenGroupWhenItemsDontExist()
        {
            const string slug = "not-found";
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroup>());

            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be($"No group found for '{slug}'");
        }

        [Fact]
        public void ShouldGetAllGroupsWithSpecifiedCategory()
        {
            // Arrange
            var testCategorySlug = "test-category-slug";
            var listOfContentfulGroups = SetupContentfulGroups(testCategorySlug);
            var listOfContentfulGroupCategories = new List<ContentfulGroupCategory> {new ContentfulGroupCategory() {Slug = testCategorySlug} };

            var listOfGroups = SetupGroups(testCategorySlug);
            var listOfGroupCategories = new List<GroupCategory> {new GroupCategory("name", testCategorySlug, "icon", "image-url.jpg")};

            // Act
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).Limit(1000).FieldEquals("fields.mapPosition[near]", "53.4058127852323,-2.15804100036621,10");
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(listOfContentfulGroups);

            _listGroupFactory.Setup(o => o.ToModel(listOfContentfulGroups)).Returns(listOfGroups);

            var categoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == categoryBuilder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(listOfContentfulGroupCategories);

            _listGroupCategoryFactory.Setup(o => o.ToModel(listOfContentfulGroupCategories)).Returns(listOfGroupCategories);


            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults(testCategorySlug, 53.40581278523235, -2.158041000366211, "name a-z"));
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
            var listOfContentfulGroups = SetupContentfulGroups(testCategorySlug);
            var listOfContentfulGroupCategories = new List<ContentfulGroupCategory> { new ContentfulGroupCategory() { Slug = "slug-that-matches-no-groups" } };

            var listOfGroups = SetupGroups(testCategorySlug);
            var listOfGroupCategories = new List<GroupCategory> { new GroupCategory("name", "slug-that-matches-no-groups", "icon", "image-url.jpg") };

            // Act
            var builder =
                new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
                    .Include(1)
                    .Limit(1000)
                    .FieldEquals("fields.mapPosition[near]", "53.4058127852323,-2.15804100036621,10");
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(listOfContentfulGroups);

            _listGroupFactory.Setup(o => o.ToModel(listOfContentfulGroups)).Returns(listOfGroups);

            var categoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == categoryBuilder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(listOfContentfulGroupCategories);

            _listGroupCategoryFactory.Setup(o => o.ToModel(listOfContentfulGroupCategories)).Returns(listOfGroupCategories);

            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults("slug-that-matches-no-groups", 53.40581278523235, -2.158041000366211, "name a-z"));
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
            var listOfContentfulGroups = SetupContentfulGroups(testCategorySlug);
            var listOfGroups = SetupGroups(testCategorySlug);

            var rawContentfulGroupCategory = new ContentfulGroupCategoryBuilder().Slug(slug).Build();
            var rawGroupCategory = new GroupCategory("name", slug, "icon", "imageUrl");

            var categorybuilder = new QueryBuilder<Entry<ContentfulGroupCategory>>().ContentTypeIs("groupCategory").Include(1);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == categorybuilder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulGroupCategory> { rawContentfulGroupCategory });

            var groupbuilder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).Limit(1000).FieldEquals("fields.mapPosition[near]", "53.4058127852323,-2.15804100036621,10");
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == groupbuilder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(listOfContentfulGroups);

            _listGroupFactory.Setup(o => o.ToModel(listOfContentfulGroups)).Returns(listOfGroups);
            _listGroupCategoryFactory.Setup(o => o.ToModel(new List<ContentfulGroupCategory> { rawContentfulGroupCategory })).Returns(new List<GroupCategory>() { rawGroupCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults(slug, 53.40581278523235, -2.158041000366211, "name a-z"));
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
            var listOfContentfulGroups = SetupContentfulGroups(testCategorySlug);
            var listOfGroups = SetupGroups(testCategorySlug);

            var rawContentfulGroupCategory = new ContentfulGroupCategoryBuilder().Slug(slug).Build();
            var rawGroupCategory = new GroupCategory("name", slug, "icon", "imageUrl");

            var categorybuilder = new QueryBuilder<Entry<ContentfulGroupCategory>>().ContentTypeIs("groupCategory").Include(1);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == categorybuilder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulGroupCategory> { rawContentfulGroupCategory });

            var groupbuilder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).Limit(1000).FieldEquals("fields.mapPosition[near]", "53.4058127852323,-2.15804100036621,10");
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == groupbuilder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(listOfContentfulGroups);

            _listGroupFactory.Setup(o => o.ToModel(listOfContentfulGroups)).Returns(listOfGroups);
            _listGroupCategoryFactory.Setup(o => o.ToModel(new List<ContentfulGroupCategory> { rawContentfulGroupCategory })).Returns(new List<GroupCategory>() { rawGroupCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults("fake-category-slug", 53.40581278523235, -2.158041000366211, "name a-z"));
            var filteredGroupResults = response.Get<GroupResults>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void ShouldReturnALocationWhenSelected()
        {
            // Arrange
            const string slug = "unit-test-GroupCategory";
            MapPosition location = new MapPosition() {Lat = 1, Lon = 1};
            var contentfulGroupWithlocation = new ContentfulGroupBuilder().Slug(slug).MapPosition(location).Build();

            var groupWithLocation = new Group("name", slug, "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, location, false);

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroup> { contentfulGroupWithlocation });

            _groupFactory.Setup(o => o.ToModel(contentfulGroupWithlocation)).Returns(groupWithLocation);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug));

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
            bool volunteering = true;
            var groupWithLocation = new Group("name", slug, "phoneNumber", "email",
                "website", "twitter", "facebook", "address", "description", "imageUrl", "thumbnailImageUrl", null, null, location, volunteering);

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroup> { contentfulGroupWithlocation });

            _groupFactory.Setup(o => o.ToModel(contentfulGroupWithlocation)).Returns(groupWithLocation);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroup(slug));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseProfile = response.Get<Group>();
            responseProfile.Volunteering.Should().Be(volunteering);
        }

        [Fact]
        public void ShouldOrderByAToZAsSpecified()
        {
            // Arrange
            var contentfulGroupcategory = new ContentfulGroupCategoryBuilder().Slug("slug").Build();
            var groupCategory = new GroupCategory("name", "slug", "icon", "image");
            var location = new MapPosition() {Lat = 5, Lon = -2};
            var contentfulGroup = new ContentfulGroupBuilder().Slug("slug").Build();
            var groupfirst = new GroupBuilder().Name("aGroup").Slug("slug1").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();
            var groupsecond = new GroupBuilder().Name("bGroup").Slug("slug2").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();
            var groupthird = new GroupBuilder().Name("cGroup").Slug("slug3").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();

            var categorybuilder = new QueryBuilder<Entry<ContentfulGroupCategory>>().ContentTypeIs("groupCategory").Include(1);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == categorybuilder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulGroupCategory> { contentfulGroupcategory });

            var groupbuilder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).Limit(1000).FieldEquals("fields.mapPosition[near]", "53.4058127852323,-2.15804100036621,10");
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == groupbuilder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroup>() { contentfulGroup });

            _listGroupFactory.Setup(o => o.ToModel(new List<ContentfulGroup>() { contentfulGroup })).Returns(new List<Group>() { groupsecond, groupfirst , groupthird });
            _listGroupCategoryFactory.Setup(o => o.ToModel(new List<ContentfulGroupCategory> { contentfulGroupcategory })).Returns(new List<GroupCategory>() { groupCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults("slug", 53.40581278523235, -2.158041000366211, "name a-z" ));
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
            var contentfulGroupcategory = new ContentfulGroupCategoryBuilder().Slug("slug").Build();
            var groupCategory = new GroupCategory("name", "slug", "icon", "image");
            var location = new MapPosition() { Lat = 5, Lon = -2 };
            var contentfulGroup = new ContentfulGroupBuilder().Slug("slug").Build();
            var groupfirst = new GroupBuilder().Name("aGroup").Slug("slug1").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();
            var groupsecond = new GroupBuilder().Name("bGroup").Slug("slug2").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();
            var groupthird = new GroupBuilder().Name("cGroup").Slug("slug3").MapPosition(location).CategoriesReference(new List<GroupCategory>() { groupCategory }).Build();

            var categorybuilder = new QueryBuilder<Entry<ContentfulGroupCategory>>().ContentTypeIs("groupCategory").Include(1);
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build() == categorybuilder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulGroupCategory> { contentfulGroupcategory });

            var groupbuilder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).Limit(1000).FieldEquals("fields.mapPosition[near]", "53.4058127852323,-2.15804100036621,10");
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build() == groupbuilder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulGroup>() { contentfulGroup });

            _listGroupFactory.Setup(o => o.ToModel(new List<ContentfulGroup>() { contentfulGroup })).Returns(new List<Group>() { groupsecond, groupfirst, groupthird });
            _listGroupCategoryFactory.Setup(o => o.ToModel(new List<ContentfulGroupCategory> { contentfulGroupcategory })).Returns(new List<GroupCategory>() { groupCategory });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetGroupResults("slug", 53.40581278523235, -2.158041000366211, "name z-a"));
            var filteredGroupResults = response.Get<GroupResults>();

            // Assert
            filteredGroupResults.Groups.First().Should().Be(groupthird);
            filteredGroupResults.Groups[1].Should().Be(groupsecond);
            filteredGroupResults.Groups[2].Should().Be(groupfirst);
        }

        [Fact]
        public void ShouldReturnNoEventsIfTheGroupIsReferencedByNone()
        {
            // Arrange
            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.group.sys.contentType.sys.id", "group").FieldEquals("fields.group.fields.slug", "slug").Include(2);

            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulEvent>());

            // Act
            var events = AsyncTestHelper.Resolve(_repository.GetGroupEvents("slug")); 

            // Assert
            events.Should().BeEmpty();
        }

        [Fact]
        public void ShouldReturnOneEventIfTheGroupIsReferencedByOne()
        {
            // Arrange
            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.group.sys.contentType.sys.id", "group").FieldEquals("fields.group.fields.slug", "slug").Include(2);

            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulEvent> { new ContentfulEventBuilder().Build() });

            _eventFactory.Setup(o => o.ToModel(It.IsAny<ContentfulEvent>())).Returns(new EventBuilder().Slug("event-slug").Build());
            // Act
            var events = AsyncTestHelper.Resolve(_repository.GetGroupEvents("slug"));

            // Assert
            events.Count.Should().Be(1);
            events[0].Slug.Should().Be("event-slug");
        }

        [Fact]
        public void ShouldReturnThreeEventsIfTheGroupIsReferencedByThree()
        {
            // Arrange
            var builder = new QueryBuilder<ContentfulEvent>().ContentTypeIs("events").FieldEquals("fields.group.sys.contentType.sys.id", "group").FieldEquals("fields.group.fields.slug", "slug").Include(2);

            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulEvent>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulEvent> { new ContentfulEventBuilder().Build(), new ContentfulEventBuilder().Build(), new ContentfulEventBuilder().Build() });

            _eventFactory.Setup(o => o.ToModel(It.IsAny<ContentfulEvent>())).Returns(new EventBuilder().Slug("event-slug").Build());
            // Act
            var events = AsyncTestHelper.Resolve(_repository.GetGroupEvents("slug"));

            // Assert
            events.Count.Should().Be(3);
            events[0].Slug.Should().Be("event-slug");
        }

        private List<ContentfulGroup> SetupContentfulGroups(string testCategorySlug)
        {
            
            var contentfulGroupCategory = new ContentfulGroupCategoryBuilder().Slug(testCategorySlug).Build();
            var contentfulGroupWithCategory = new ContentfulGroupBuilder().Slug("slug-with-categories").CategoriesReference(
                new List<Entry<ContentfulGroupCategory>>
                {
                    new Entry<ContentfulGroupCategory>
                    {
                        Fields = contentfulGroupCategory,
                        SystemProperties = new SystemProperties() { Type = "Entry" }
                    }
                })
                .Build();
            var contentfulGroupWithoutCategory = new ContentfulGroupBuilder().Slug("slug-without-categories").Build();
            
            return new List<ContentfulGroup>
            {
                contentfulGroupWithCategory,
                contentfulGroupWithoutCategory
            };
        }

        private List<Group> SetupGroups(string testCategorySlug)
        {
            var GroupCategory = new GroupCategory("name", testCategorySlug, "icon", "imagueUrl");
            var GroupWithCategory = new GroupBuilder().Slug("slug-with-categories").CategoriesReference(
                new List<GroupCategory>()
                {
                    GroupCategory
                })
                .Build();

            var GroupWithoutCategory = new GroupBuilder().Slug("slug-without-categories").Build();

            return new List<Group>
            {
                GroupWithCategory,
                GroupWithoutCategory
            };
        }



    }
    
}