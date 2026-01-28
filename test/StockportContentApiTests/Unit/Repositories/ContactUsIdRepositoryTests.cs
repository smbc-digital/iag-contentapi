namespace StockportContentApiTests.Unit.Repositories;

public class ContactUsIdRepositoryTests
{
    private readonly ContactUsIdRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();

    public ContactUsIdRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        ContactUsIdContentfulFactory contentfulFactory = new();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(config))
            .Returns(_contentfulClient.Object);

        _repository = new ContactUsIdRepository(config, contentfulFactory, contentfulClientManager.Object);
    }

    [Fact]
    public async Task ItGetsContactUsId()
    {
        // Arrange
        const string slug = "unit-test-showcase";

        ContentfulContactUsId rawContactUsId = new();
        ContentfulCollection<ContentfulContactUsId> collection = new()
        {
            Items = new List<ContentfulContactUsId> { rawContactUsId }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulContactUsId>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.GetContactUsIds(slug, "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ItGetsContactUsIdFromSlug()
    {
        // Arrange
        ContentfulContactUsId rawContactUs = new() { Slug = "test-slug" };
        ContentfulCollection<ContentfulContactUsId> collection = new()
        {
            Items = new List<ContentfulContactUsId> { rawContactUs }
        };

        QueryBuilder<ContentfulContactUsId> builder = new QueryBuilder<ContentfulContactUsId>()
            .ContentTypeIs("contactUsId")
            .FieldEquals("fields.slug", "test-slug")
            .Include(1);

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulContactUsId>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.GetContactUsIds("test-slug", "tagId");
        ContactUsId model = response.Get<ContactUsId>();

        // Assert
        Assert.Equal(rawContactUs.Slug, model.Slug);
    }
}