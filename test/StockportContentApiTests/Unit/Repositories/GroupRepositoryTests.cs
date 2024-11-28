using StockportContentApi.Config;
using StockportContentApi.Constants;

namespace StockportContentApiTests.Unit.Repositories;

public class GroupRepositoryTests
{
    private readonly Mock<ICache> _cacheWrapper;
    private readonly Mock<IContentfulClient> _client;
    private readonly Mock<IConfiguration> _configuration;
    private readonly CacheKeyConfig _cacheKeyconfig;
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
    private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory;
    private readonly EventRepository _eventRepository;
    private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _groupCategoryFactory;
    private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
    private readonly Mock<IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>> _groupHomepageContentfulFactory;
    private readonly GroupRepository _repository;
    private readonly Mock<ITimeProvider> _timeProvider;

    public GroupRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
        .Build();


        _cacheKeyconfig = new CacheKeyConfig("test")
            .Add("TEST_EventsCacheKey", "testEventsCacheKey")
            .Add("TEST_NewsCacheKey", "testNewsCacheKey")
            .Build();

        _groupFactory = new();
        _eventFactory = new();
        _eventHomepageFactory = new();

        _timeProvider = new();
        _groupCategoryFactory = new();
        _groupHomepageContentfulFactory = new();

        _cacheWrapper = new();
        _configuration = new();
        _configuration.Setup(_ => _["redisExpiryTimes:Groups"]).Returns("60");
        _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");
        Mock<IContentfulClientManager> contentfulClientManager = new();
        _client = new();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

        _eventRepository = new(config, _cacheKeyconfig, contentfulClientManager.Object, _timeProvider.Object, _eventFactory.Object,
            _eventHomepageFactory.Object, _cacheWrapper.Object, _configuration.Object);
        _repository = new(config, contentfulClientManager.Object, _timeProvider.Object, _groupFactory.Object,
            _groupCategoryFactory.Object, _groupHomepageContentfulFactory.Object, _eventRepository,
            _cacheWrapper.Object, _configuration.Object);
    }

    [Fact]
    public void GetsGroupForGroupSlug()
    {
        const string slug = "group_slug";
        ContentfulGroup contentfulGroup = new ContentfulGroupBuilder().Slug(slug).Build();
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroup }
        };

        Group group = new GroupBuilder().Build();
        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build().Equals(builder.Build())),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
                It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<ContentfulEvent>());

        _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, false));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Group responseGroup = response.Get<Group>();
        responseGroup.Should().BeEquivalentTo(group);
    }

    [Fact]
    public void GetsGroupWhenHiddenDatesAreInThePast()
    {
        // Arrange
        const string slug = "group_slug";
        ContentfulGroup contentfulGroup = new ContentfulGroupBuilder().Slug(slug)
            .DateHiddenFrom(DateTime.Now.AddDays(-3)).DateHiddenTo(DateTime.Now.AddDays(-1)).Build();
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroup }
        };

        Group group = new GroupBuilder().Slug("group_slug").DateHiddenFrom(DateTime.Now.AddDays(-3))
            .DateHiddenTo(DateTime.Now.AddDays(-1)).Build();

        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build().Equals(builder.Build())),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
                It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<ContentfulEvent>());

        _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Group responseGroup = response.Get<Group>();
        responseGroup.Should().BeEquivalentTo(group);
    }

    [Fact]
    public void GetsGroupWhenHiddenDatesAreInTheFuture()
    {
        // Arrange
        const string slug = "group_slug";
        ContentfulGroup contentfulGroup = new ContentfulGroupBuilder().Slug(slug)
            .DateHiddenFrom(DateTime.Now.AddDays(1)).DateHiddenTo(DateTime.Now.AddDays(3)).Build();
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroup }
        };

        Group group = new GroupBuilder().Slug("group_slug").DateHiddenFrom(DateTime.Now.AddDays(1))
            .DateHiddenTo(DateTime.Now.AddDays(3)).Build();

        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build().Equals(builder.Build())),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
                It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<ContentfulEvent>());

        _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Group responseGroup = response.Get<Group>();
        responseGroup.Should().BeEquivalentTo(group);
    }

    [Fact]
    public void DoesntGetGroupWhenHiddenDatesAreNow()
    {
        // Arrange
        const string slug = "group_slug";
        ContentfulGroup contentfulGroup = new ContentfulGroupBuilder().Slug(slug)
            .DateHiddenFrom(DateTime.Now.AddDays(-3)).DateHiddenTo(DateTime.Now.AddDays(3)).Build();
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroup }
        };

        Group group = new GroupBuilder().Slug("group_slug").DateHiddenFrom(DateTime.Now.AddDays(-3))
            .DateHiddenTo(DateTime.Now.AddDays(3)).Build();

        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build().Equals(builder.Build())),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
                It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<ContentfulEvent>());

        _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        Group responseGroup = response.Get<Group>();
        responseGroup.Should().BeNull();
    }

    [Fact]
    public void DoesntGetGroupWhenHiddenDateFromIsInThePastAndHiddenDateToIsNull()
    {
        // Arrange
        const string slug = "group_slug";
        ContentfulGroup contentfulGroup =
            new ContentfulGroupBuilder().Slug(slug).DateHiddenFrom(DateTime.Now.AddDays(-3)).Build();
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroup }
        };

        Group group = new GroupBuilder().Slug("group_slug").DateHiddenFrom(DateTime.Now.AddDays(-3)).Build();

        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build().Equals(builder.Build())),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
                It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<ContentfulEvent>());

        _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        Group responseGroup = response.Get<Group>();
        responseGroup.Should().BeNull();
    }

    [Fact]
    public void DoesntGetGroupWhenHiddenDateToIsInTheFutureAndHiddenDateFromIsNull()
    {
        // Arrange
        const string slug = "group_slug";
        ContentfulGroup contentfulGroup =
            new ContentfulGroupBuilder().Slug(slug).DateHiddenTo(DateTime.Now.AddDays(3)).Build();
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroup }
        };

        Group group = new GroupBuilder().Slug("group_slug").DateHiddenTo(DateTime.Now.AddDays(3)).Build();

        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build().Equals(builder.Build())),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
                It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<ContentfulEvent>());

        _groupFactory.Setup(o => o.ToModel(contentfulGroup)).Returns(group);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, true));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        Group responseGroup = response.Get<Group>();
        responseGroup.Should().BeNull();
    }

    [Fact]
    public void Return404WhenGroupWhenItemsDontExist()
    {
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup>()
        };

        const string slug = "not-found";
        _client.Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulGroup>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, false));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Error.Should().Be($"No group found for '{slug}'");
    }

    [Fact]
    public async void ShouldGetAllGroupsWithSpecifiedCategory()
    {
        // Arrange
        string testCategorySlug = "test-category-slug";
        List<ContentfulGroup> listOfContentfulGroups =
            SetupMockFactoriesAndGetContentfulGroupsForCollection(testCategorySlug);
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = listOfContentfulGroups
        };
        List<GroupCategory> listOfGroupCategories = new() { new("name", testCategorySlug, "icon", "image-url.jpg") };

        // Act
        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1)
            .FieldEquals("fields.mapPosition[near]", $"{Groups.StockportLatitude},{Groups.StockportLongitude},10");
        _client
            .Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulGroup>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        QueryBuilder<ContentfulGroupCategory> categoryBuilder =
            new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("group-categories")),
                It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(listOfGroupCategories);

        HttpResponse response = await _repository.GetGroupResults(new(), testCategorySlug);
        GroupResults filteredGroupResults = response.Get<GroupResults>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        filteredGroupResults.Groups.Count.Should().Be(1);
        filteredGroupResults.Groups[0].Slug.Should().Be("slug-with-categories");
    }

    [Fact]
    public async void ShouldReturnEmptyListIfNoGroupsMatchTheCategory()
    {
        // Arrange
        string testCategorySlug = "test-category-slug";
        List<ContentfulGroup> listOfContentfulGroups =
            SetupMockFactoriesAndGetContentfulGroupsForCollection(testCategorySlug);
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = listOfContentfulGroups
        };
        List<ContentfulGroupCategory> listOfContentfulGroupCategories =
            new() { new() { Slug = "slug-that-matches-no-groups" } };
        List<GroupCategory> listOfGroupCategories =
            new() { new("name", "slug-that-matches-no-groups", "icon", "image-url.jpg") };

        // Act
        _client
            .Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulGroup>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("group-categories")),
                It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(listOfGroupCategories);

        HttpResponse response = await _repository.GetGroupResults(new() { Category = "slug-that-matches-no-groups" },
            "slug-that-matches-no-groups");
        GroupResults filteredGroupResults = response.Get<GroupResults>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        filteredGroupResults.Groups.Count.Should().Be(0);
    }

    [Fact]
    public void ShouldReturnAListOfAListOfCategories()
    {
        // Arrange
        const string slug = "unit-test-GroupCategory";
        string testCategorySlug = "test-category-slug";
        List<ContentfulGroup> listOfContentfulGroups =
            SetupMockFactoriesAndGetContentfulGroupsForCollection(testCategorySlug);
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = listOfContentfulGroups
        };

        ContentfulGroupCategory rawContentfulGroupCategory = new ContentfulGroupCategoryBuilder().Slug(slug).Build();
        GroupCategory rawGroupCategory = new("name", slug, "icon", "imageUrl");

        _client
            .Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulGroup>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);
        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("group-categories")),
                It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<GroupCategory> { rawGroupCategory });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroupResults(new(), slug));
        GroupResults filteredGroupResults = response.Get<GroupResults>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        filteredGroupResults.Categories.Count.Should().Be(1);
    }

    [Fact]
    public void ShouldReturn404IfSpecifiedCategoryDoesntExist()
    {
        // Arrange
        const string slug = "unit-test-GroupCategory";
        string testCategorySlug = "test-category-slug";
        List<ContentfulGroup> listOfContentfulGroups =
            SetupMockFactoriesAndGetContentfulGroupsForCollection(testCategorySlug);
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = listOfContentfulGroups
        };

        ContentfulGroupCategory rawContentfulGroupCategory = new ContentfulGroupCategoryBuilder().Slug(slug).Build();
        GroupCategory rawGroupCategory = new("name", slug, "icon", "imageUrl");

        string builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1)
            .FieldEquals("fields.mapPosition[near]", $"{Groups.StockportLatitude},{Groups.StockportLongitude},10")
            .Build();
        _client.Setup(o => o.GetEntries<ContentfulGroup>(It.Is<string>(q => q.Contains(builder)),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);
        _cacheWrapper.Setup(o =>
                o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("group-categories")),
                    It.IsAny<Func<Task<List<GroupCategory>>>>(),
                    It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<GroupCategory> { rawGroupCategory });

        string noLatLngBuilder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1).Build();
        _client.Setup(o => o.GetEntries<ContentfulGroup>(It.Is<string>(q => q.Contains(noLatLngBuilder)),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroupResults(new()
        {
            Category = "test"
        }, "fake-category-slug"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void ShouldReturnALocationWhenSelected()
    {
        // Arrange
        const string slug = "unit-test-GroupCategory";
        MapPosition location = new() { Lat = 1, Lon = 1 };
        ContentfulGroup contentfulGroupWithlocation =
            new ContentfulGroupBuilder().Slug(slug).MapPosition(location).Build();
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroupWithlocation }
        };

        Group groupWithLocation = new GroupBuilder().MapPosition(location).Slug(slug).Build();

        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build().Equals(builder.Build())),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _groupFactory.Setup(o => o.ToModel(contentfulGroupWithlocation)).Returns(groupWithLocation);
        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
                It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<ContentfulEvent>());

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, false));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Group responseProfile = response.Get<Group>();
        responseProfile.MapPosition.Should().BeEquivalentTo(location);
    }

    [Fact]
    public void ShouldReturnVolunteeringWhenSelected()
    {
        // Arrange
        const string slug = "unit-test-GroupCategory";
        MapPosition location = new() { Lat = 1, Lon = 1 };
        ContentfulGroup contentfulGroupWithlocation =
            new ContentfulGroupBuilder().Slug(slug).MapPosition(location).Build();
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroupWithlocation }
        };

        bool volunteering = true;
        Group groupWithLocation = new GroupBuilder().Volunteering(volunteering).Slug(slug).Build();

        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build().Equals(builder.Build())),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("testEventsCacheKey-all")),
                It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<ContentfulEvent>());
        _groupFactory.Setup(o => o.ToModel(contentfulGroupWithlocation)).Returns(groupWithLocation);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroup(slug, false));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Group responseProfile = response.Get<Group>();
        responseProfile.Volunteering.Should().Be(volunteering);
    }

    [Fact]
    public void ShouldOrderByAToZAsSpecified()
    {
        // Arrange
        GroupCategory groupCategory = new("name", "slug", "icon", "image");
        MapPosition location = new() { Lat = 5, Lon = -2 };

        ContentfulGroup contentfulGroupFirst = new ContentfulGroupBuilder().Slug("slug1").Build();
        ContentfulGroup contentfulGroupSecond = new ContentfulGroupBuilder().Slug("slug2").Build();
        ContentfulGroup contentfulGroupThird = new ContentfulGroupBuilder().Slug("slug3").Build();

        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroupFirst, contentfulGroupSecond, contentfulGroupThird }
        };

        Group groupfirst = new GroupBuilder().Name("aGroup").Slug("slug1").MapPosition(location)
            .CategoriesReference(new() { groupCategory }).Build();
        Group groupsecond = new GroupBuilder().Name("bGroup").Slug("slug2").MapPosition(location)
            .CategoriesReference(new() { groupCategory }).Build();
        Group groupthird = new GroupBuilder().Name("cGroup").Slug("slug3").MapPosition(location)
            .CategoriesReference(new() { groupCategory }).Build();

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("group-categories")),
                It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<GroupCategory> { groupCategory });

        _client
            .Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulGroup>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _groupFactory.Setup(o => o.ToModel(contentfulGroupFirst)).Returns(groupfirst);
        _groupFactory.Setup(o => o.ToModel(contentfulGroupSecond)).Returns(groupsecond);
        _groupFactory.Setup(o => o.ToModel(contentfulGroupThird)).Returns(groupthird);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroupResults(new(), "slug"));
        GroupResults filteredGroupResults = response.Get<GroupResults>();

        // Assert
        filteredGroupResults.Groups.First().Should().Be(groupfirst);
        filteredGroupResults.Groups[1].Should().Be(groupsecond);
        filteredGroupResults.Groups[2].Should().Be(groupthird);
    }

    [Fact]
    public void ShouldOrderByZToAAsSpecified()
    {
        // Arrange
        GroupCategory groupCategory = new("name", "slug", "icon", "image");
        MapPosition location = new() { Lat = 5, Lon = -2 };
        ContentfulGroup contentfulGroupFirst = new ContentfulGroupBuilder().Slug("slug1").Build();
        ContentfulGroup contentfulGroupSecond = new ContentfulGroupBuilder().Slug("slug2").Build();
        ContentfulGroup contentfulGroupThird = new ContentfulGroupBuilder().Slug("slug3").Build();
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = new List<ContentfulGroup> { contentfulGroupFirst, contentfulGroupSecond, contentfulGroupThird }
        };

        Group groupfirst = new GroupBuilder().Name("aGroup").Slug("slug1").MapPosition(location)
            .CategoriesReference(new() { groupCategory }).Build();
        Group groupsecond = new GroupBuilder().Name("bGroup").Slug("slug2").MapPosition(location)
            .CategoriesReference(new() { groupCategory }).Build();
        Group groupthird = new GroupBuilder().Name("cGroup").Slug("slug3").MapPosition(location)
            .CategoriesReference(new() { groupCategory }).Build();

        _client
            .Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulGroup>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _groupFactory.Setup(o => o.ToModel(contentfulGroupFirst)).Returns(groupfirst);
        _groupFactory.Setup(o => o.ToModel(contentfulGroupSecond)).Returns(groupsecond);
        _groupFactory.Setup(o => o.ToModel(contentfulGroupThird)).Returns(groupthird);

        _cacheWrapper
            .Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals("group-categories")),
                It.IsAny<Func<Task<List<GroupCategory>>>>(), It.Is<int>(s => s.Equals(60))))
            .ReturnsAsync(new List<GroupCategory> { groupCategory });

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroupResults(new()
        {
            Category = string.Empty,
            GetInvolved = string.Empty,
            Latitude = 0.0d,
            Location = string.Empty,
            Longitude = 0.0d,
            Order = "name z-a",
            Organisation = string.Empty,
            SubCategories = string.Empty,
            Tag = string.Empty,
            Tags = string.Empty
        }, "slug"));
        GroupResults filteredGroupResults = response.Get<GroupResults>();

        // Assert
        filteredGroupResults.Groups.First().Should().Be(groupthird);
        filteredGroupResults.Groups[1].Should().Be(groupsecond);
        filteredGroupResults.Groups[2].Should().Be(groupfirst);
    }

    [Fact]
    public void ShouldReturnListOfGroupsWhereAdministratorMatchesEmailAddress()
    {
        // Arrange
        string emailAddress = "test@test.com";
        List<ContentfulGroup> contentfulGroupsReturned = new();
        ContentfulGroup correctContentfulGroup = new ContentfulGroupBuilder()
            .GroupAdministrators(new() { Items = new() { new() { Email = emailAddress, Permission = "A" } } }).Build();
        contentfulGroupsReturned.Add(new ContentfulGroupBuilder()
            .GroupAdministrators(new() { Items = new() { new() { Email = "bill@yahoo.com", Permission = "E" } } })
            .Build());
        contentfulGroupsReturned.Add(correctContentfulGroup);
        contentfulGroupsReturned.Add(new ContentfulGroupBuilder()
            .GroupAdministrators(new() { Items = new() { new() { Email = "fred@msn.com", Permission = "A" } } })
            .Build());
        contentfulGroupsReturned.Add(new ContentfulGroupBuilder().GroupAdministrators(new()
        { Items = new() { new() { Email = "jerry@gmail.com", Permission = "A" } } }).Build());
        ContentfulCollection<ContentfulGroup> collection = new()
        {
            Items = contentfulGroupsReturned
        };

        Group groupReturned = new GroupBuilder()
            .GroupAdministrators(new() { Items = new() { new() { Email = emailAddress, Permission = "A" } } }).Build();

        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldExists("fields.groupAdministrators")
            .Include(1)
            .Limit(ContentfulQueryValues.LIMIT_MAX);
        _client.Setup(
            o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroup>>(q => q.Build().Equals(builder.Build())),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _groupFactory.Setup(o => o.ToModel(correctContentfulGroup)).Returns(groupReturned);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetAdministratorsGroups(emailAddress));
        List<Group> filteredGroups = response.Get<List<Group>>();

        // Assert
        filteredGroups.Count.Should().Be(1);
    }

    [Fact]
    public void ShouldReturnContenfulGroupHomepage()
    {
        ContentfulGroupHomepage contenfulHomepage = new ContentfulGroupHomepageBuilder().Build();
        ContentfulCollection<ContentfulGroupHomepage> collection = new()
        {
            Items = new List<ContentfulGroupHomepage> { contenfulHomepage }
        };

        GroupHomepage groupHomepage = new("title", "slug", "metaDescription", "image-url.jpg", string.Empty, null, null,
            null, null, string.Empty, string.Empty, string.Empty, string.Empty, new NullEventBanner());

        QueryBuilder<ContentfulGroupHomepage> builder =
            new QueryBuilder<ContentfulGroupHomepage>().ContentTypeIs("groupHomepage").Include(1);

        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupHomepage>>(q => q.Build().Equals(builder.Build())),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _groupHomepageContentfulFactory.Setup(o => o.ToModel(contenfulHomepage)).Returns(groupHomepage);

        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroupHomepage());
        GroupHomepage homepage = response.Get<GroupHomepage>();

        homepage.BackgroundImage.Should().Be(contenfulHomepage.BackgroundImage.File.Url);
        homepage.Title.Should().Be(contenfulHomepage.Title);
        homepage.Slug.Should().Be(contenfulHomepage.Slug);
    }

    private List<ContentfulGroup> SetupMockFactoriesAndGetContentfulGroupsForCollection(string testCategorySlug)
    {
        ContentfulGroupCategory contentfulGroupCategory =
            new ContentfulGroupCategoryBuilder().Slug(testCategorySlug).Build();
        ContentfulGroup contentfulGroupWithCategory = new ContentfulGroupBuilder().Slug("slug-with-categories")
            .CategoriesReference(
                new() { contentfulGroupCategory }).Build();
        ContentfulGroup contentfulGroupWithoutCategory =
            new ContentfulGroupBuilder().Slug("slug-without-categories").Build();

        GroupCategory groupCategory = new("name", testCategorySlug, "icon", "imagueUrl");
        Group groupWithCategory = new GroupBuilder().Slug("slug-with-categories").CategoriesReference(
                new()
                {
                    groupCategory
                })
            .Build();

        Group groupWithoutCategory = new GroupBuilder().Slug("slug-without-categories").Build();

        _groupFactory.Setup(o => o.ToModel(contentfulGroupWithCategory)).Returns(groupWithCategory);
        _groupFactory.Setup(o => o.ToModel(contentfulGroupWithoutCategory)).Returns(groupWithoutCategory);

        return new()
        {
            contentfulGroupWithCategory,
            contentfulGroupWithoutCategory
        };
    }
}