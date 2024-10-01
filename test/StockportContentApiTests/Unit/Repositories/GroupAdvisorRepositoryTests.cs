namespace StockportContentApiTests.Unit.Repositories;

public class GroupAdvisorRepositoryTests
{
    readonly Mock<IContentfulClientManager> _contentfulClientManager = new();
    readonly Mock<IContentfulClient> _client = new();
    readonly Mock<IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>> _contentfulFactory = new();
    readonly GroupAdvisorRepository _repository;

    public GroupAdvisorRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

        _repository = new GroupAdvisorRepository(config, _contentfulClientManager.Object, _contentfulFactory.Object);
    }

    [Fact]
    public void ShouldGetAdvisorsForAGroup()
    {
        // Arrange
        string query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").Include(1).Build();
        ContentfulCollection<ContentfulGroupAdvisor> collection = new();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("test-group").Build() }).Build(),
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("test-group").Build() }).Build(),
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("not-a-group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build().Equals(query)), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug.Equals("test-group")))))
                          .Returns(new GroupAdvisorBuilder().Groups(new List<string>() { "test-group" }).Build());
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug.Equals("not-a-group")))))
                          .Returns(new GroupAdvisorBuilder().Groups(new List<string>() { "not-a-group" }).Build());

        // Act
        List<GroupAdvisor> result = AsyncTestHelper.Resolve(_repository.GetAdvisorsByGroup("test-group"));

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ShouldGetAdvisorsGroupByEmail()
    {
        // Arrange
        string query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        ContentfulCollection<ContentfulGroupAdvisor> collection = new();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build().Equals(query)), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroupAdvisor>())).Returns(new GroupAdvisorBuilder().Build());

        // Act
        GroupAdvisor result = AsyncTestHelper.Resolve(_repository.Get("email"));

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
        string query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        ContentfulCollection<ContentfulGroupAdvisor> collection = new();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("test-group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build().Equals(query)), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug.Equals("test-group")))))
                          .Returns(new GroupAdvisorBuilder().Groups(new List<string>() { "test-group" }).Build());

        // Act
        bool result = AsyncTestHelper.Resolve(_repository.CheckIfUserHasAccessToGroupBySlug("test-group", "email"));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldReturnFalseIfGroupIsntInListForEmail()
    {
        // Arrange
        string query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        ContentfulCollection<ContentfulGroupAdvisor> collection = new();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build().Equals(query)), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug.Equals("group")))))
                          .Returns(new GroupAdvisorBuilder().Groups(new List<string>() { "group" }).Build());

        // Act
        bool result = AsyncTestHelper.Resolve(_repository.CheckIfUserHasAccessToGroupBySlug("test-group", "email"));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnTrueIfUserHasGlobalAccessAndDoesntHaveGroupInList()
    {
        // Arrange
        string query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        ContentfulCollection<ContentfulGroupAdvisor> collection = new();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().GlobalAccess(true).ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build().Equals(query)), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug.Equals("group")))))
                          .Returns(new GroupAdvisorBuilder().GlobalAccess(true).Groups(new List<string>() { "group" }).Build());

        // Act
        bool result = AsyncTestHelper.Resolve(_repository.CheckIfUserHasAccessToGroupBySlug("test-group", "email"));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldReturnTrueIfUserHasGlobalAccessAndHasGroupInList()
    {
        // Arrange
        string query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
        ContentfulCollection<ContentfulGroupAdvisor> collection = new();
        collection.Items = new List<ContentfulGroupAdvisor>()
        {
            new ContentfulGroupAdvisorBuilder().GlobalAccess(true).ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("group").Build() }).Build()
        };

        // Mock
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build().Equals(query)), It.IsAny<CancellationToken>()))
               .ReturnsAsync(collection);
        _contentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulGroupAdvisor>(g => g.Groups.ToList().Exists(p => p.Slug.Equals("group")))))
                          .Returns(new GroupAdvisorBuilder().GlobalAccess(true).Groups(new List<string>() { "group" }).Build());

        // Act
        bool result = AsyncTestHelper.Resolve(_repository.CheckIfUserHasAccessToGroupBySlug("group", "email"));

        // Assert
        result.Should().BeTrue();
    }
}
