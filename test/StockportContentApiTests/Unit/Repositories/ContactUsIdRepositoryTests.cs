namespace StockportContentApiTests.Unit.Repositories;

public class ContactUsIdRepositoryTests
{
    private readonly Mock<IHttpClient> _httpClient;
    private readonly ContactUsIdRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulContactUsId, ContactUsId>> _contactUsIdFactory;

    public ContactUsIdRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _httpClient = new Mock<IHttpClient>();
        _contactUsIdFactory = new Mock<IContentfulFactory<ContentfulContactUsId, ContactUsId>>();

        ContactUsIdContentfulFactory contentfulFactory = new();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        _repository = new ContactUsIdRepository(config, contentfulFactory, contentfulClientManager.Object);
    }

    [Fact]
    public void ItGetsContactUsId()
    {
        // Arrange
        const string slug = "unit-test-showcase";

        ContentfulContactUsId rawContactUsId = new();
        ContentfulCollection<ContentfulContactUsId> collection = new();
        collection.Items = new List<ContentfulContactUsId> { rawContactUsId };

        _contentfulClient.Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulContactUsId>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetContactUsIds(slug));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void ItGetsContactUsIdFromSlug()
    {
        // Arrange
        const string slug = "test-slug";
        ContentfulContactUsId rawContactUs = new() { Slug = slug };
        ContentfulCollection<ContentfulContactUsId> collection = new();
        collection.Items = new List<ContentfulContactUsId> { rawContactUs };

        // Act
        QueryBuilder<ContentfulContactUsId> builder = new QueryBuilder<ContentfulContactUsId>().ContentTypeIs("contactUsId").FieldEquals("fields.slug", slug).Include(1);

        _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulContactUsId>>(q => q.Build().Equals(builder.Build())), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetContactUsIds(slug));
        ContactUsId model = response.Get<ContactUsId>();

        // Assert
        model.Slug.Should().Be(rawContactUs.Slug);
    }
}
