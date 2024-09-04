namespace StockportContentApiTests.Unit.Repositories;

public class ContactUsIdRepositoryTest
{
    private readonly Mock<IHttpClient> _httpClient;
    private readonly ContactUsIdRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulContactUsId, ContactUsId>> _contactUsIdFactory;

    public ContactUsIdRepositoryTest()
    {
        var config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _httpClient = new Mock<IHttpClient>();
        _contactUsIdFactory = new Mock<IContentfulFactory<ContentfulContactUsId, ContactUsId>>();

        var contentfulFactory = new ContactUsIdContentfulFactory();

        var contentfulClientManager = new Mock<IContentfulClientManager>();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        _repository = new ContactUsIdRepository(config, contentfulFactory, contentfulClientManager.Object);
    }

    [Fact]
    public void ItGetsContactUsId()
    {
        // Arrange
        const string slug = "unit-test-showcase";

        var rawContactUsId = new ContentfulContactUsId();
        var collection = new ContentfulCollection<ContentfulContactUsId>();
        collection.Items = new List<ContentfulContactUsId> { rawContactUsId };

        _contentfulClient.Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulContactUsId>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetContactUsIds(slug));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void ItGetsContactUsIdFromSlug()
    {
        // Arrange
        const string slug = "test-slug";
        var rawContactUs = new ContentfulContactUsId() { Slug = slug };
        var collection = new ContentfulCollection<ContentfulContactUsId>();
        collection.Items = new List<ContentfulContactUsId> { rawContactUs };

        // Act
        var builder = new QueryBuilder<ContentfulContactUsId>().ContentTypeIs("contactUsId").FieldEquals("fields.slug", slug).Include(1);

        _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulContactUsId>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        var response = AsyncTestHelper.Resolve(_repository.GetContactUsIds(slug));
        var model = response.Get<ContactUsId>();

        // Assert
        model.Slug.Should().Be(rawContactUs.Slug);
    }
}
