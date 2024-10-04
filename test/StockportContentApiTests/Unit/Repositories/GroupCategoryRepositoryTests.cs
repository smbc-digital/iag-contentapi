namespace StockportContentApiTests.Unit.Repositories;

public class GroupCategoryRepositoryTests
{
    private readonly GroupCategoryRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _contentfulGroupCategoryFactory;

    public GroupCategoryRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        _contentfulGroupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        _repository = new GroupCategoryRepository(config, _contentfulGroupCategoryFactory.Object, contentfulClientManager.Object);
    }

    [Fact]
    public void ItGetsGroupCategories()
    {
        // Arrange
        const string slug = "unit-test-GroupCategory";

        ContentfulGroupCategory rawGroupCategory = new ContentfulGroupCategoryBuilder().Slug(slug).Name("name").Build();
        ContentfulCollection<ContentfulGroupCategory> collection = new()
        {
            Items = new List<ContentfulGroupCategory> { rawGroupCategory }
        };

        QueryBuilder<ContentfulGroupCategory> builder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory");
        _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulGroupCategory>>(q => q.Build().Equals(builder.Build())), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _contentfulGroupCategoryFactory.Setup(_ => _.ToModel(rawGroupCategory))
            .Returns(new GroupCategory("name", "slug", "icon", "imageUrl"));

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroupCategories());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void ShouldReturnNotFoundIfNoGroupCategoryFound()
    {
        ContentfulCollection<ContentfulGroupCategory> collection = new()
        {
            Items = new List<ContentfulGroupCategory>()
        };

        _contentfulClient.Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulGroupCategory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetGroupCategories());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
