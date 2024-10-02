namespace StockportContentApiTests.Unit.Repositories;

public class GroupAdvisorRepositoryTests
{
    Mock<IContentfulClientManager> _contentfulClientManager = new Mock<IContentfulClientManager>();
    Mock<IContentfulClient> _client = new Mock<IContentfulClient>();
    Mock<IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>> _contentfulFactory = new Mock<IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>>();
    GroupAdvisorRepository _reporisory;

    public GroupAdvisorRepositoryTests()
    {
        var config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

        _reporisory = new GroupAdvisorRepository(config, _contentfulClientManager.Object, _contentfulFactory.Object);
    }

    [Fact]
    public void ShouldGetAdvisorsForAGroup()
    {
        // Arrange
        var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").Include(1).Build();
        var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("test-group").Build() }).Build(),
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("test-group").Build() }).Build(),
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("not-a-group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug == "test-group"))))
                          .Returns(new GroupAdvisorBuilder().Groups(new List<string>() { "test-group" }).Build());
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug == "not-a-group"))))
                          .Returns(new GroupAdvisorBuilder().Groups(new List<string>() { "not-a-group" }).Build());

        // Act
        var result = AsyncTestHelper.Resolve(_reporisory.GetAdvisorsByGroup("test-group"));

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ShouldGetAdvisorsGroupByEmail()
    {
        // Arrange
        var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroupAdvisor>())).Returns(new GroupAdvisorBuilder().Build());

        // Act
        var result = AsyncTestHelper.Resolve(_reporisory.Get("email"));

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("name");
        result.EmailAddress.Should().Be("email");
        result.HasGlobalAccess.Should().Be(false);
    }

    [Fact]
    public void ShouldReturnTrueForIfGroupIsInListForEmail()
    {
        // Arrange
        var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("test-group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug == "test-group"))))
                          .Returns(new GroupAdvisorBuilder().Groups(new List<string>() { "test-group" }).Build());

        // Act
        var result = AsyncTestHelper.Resolve(_reporisory.CheckIfUserHasAccessToGroupBySlug("test-group", "email"));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldReturnFalseIfGroupIsntInListForEmail()
    {
        // Arrange
        var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug == "group"))))
                          .Returns(new GroupAdvisorBuilder().Groups(new List<string>() { "group" }).Build());

        // Act
        var result = AsyncTestHelper.Resolve(_reporisory.CheckIfUserHasAccessToGroupBySlug("test-group", "email"));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnTrueIfUserHasGlobalAccessAndDoesntHaveGroupInList()
    {
        // Arrange
        var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().GlobalAccess(true).ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug == "group"))))
                          .Returns(new GroupAdvisorBuilder().GlobalAccess(true).Groups(new List<string>() { "group" }).Build());

        // Act
        var result = AsyncTestHelper.Resolve(_reporisory.CheckIfUserHasAccessToGroupBySlug("test-group", "email"));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldReturnTrueIfUserHasGlobalAccessAndHasGroupInList()
    {
        // Arrange
        var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().GlobalAccess(true).ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug == "group"))))
                          .Returns(new GroupAdvisorBuilder().GlobalAccess(true).Groups(new List<string>() { "group" }).Build());

        // Act
        var result = AsyncTestHelper.Resolve(_reporisory.CheckIfUserHasAccessToGroupBySlug("group", "email"));

        // Assert
        result.Should().BeTrue();
    }
}
