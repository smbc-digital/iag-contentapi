namespace StockportContentApiTests.Unit.Repositories;

public class ContactUsAreaRepositoryTests
{
    private readonly ContactUsAreaRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _mockSubitemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _mockCrumbFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _mockAlertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>> _mockContactUsCategoryFactory = new();

    private readonly Mock<ITimeProvider> _timeprovider = new();

    public ContactUsAreaRepositoryTests()
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
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        ContactUsAreaContentfulFactory contentfulFactory = new(_mockSubitemFactory.Object,
            _mockCrumbFactory.Object,
            _timeprovider.Object,
            _mockAlertFactory.Object,
            _mockContactUsCategoryFactory.Object);

        _repository = new ContactUsAreaRepository(config, contentfulClientManager.Object, contentfulFactory);
    }

    [Fact]
    public void ItGetsContactUsArea()
    {
        // Arrange
        const string slug = "contactusarea";

        ContentfulCollection<ContentfulContactUsArea> collection = new();
        ContentfulContactUsArea rawContactUsArea = new ContentfulContactUsAreaBuilder().Slug(slug).Build();
        collection.Items = new List<ContentfulContactUsArea> { rawContactUsArea };

        QueryBuilder<ContentfulContactUsArea> builder = new QueryBuilder<ContentfulContactUsArea>().ContentTypeIs("contactUsArea").Include(3);

        _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulContactUsArea>>(q => q.Build().Equals(builder.Build())), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetContactUsArea());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void ItReturnsBreadcrumbs()
    {
        // Arrange
        const string slug = "contactusarea";
        Crumb crumb = new("title", "slug", "type");
        ContentfulCollection<ContentfulContactUsArea> collection = new();
        ContentfulContactUsArea rawContactUsArea = new ContentfulContactUsAreaBuilder().Slug(slug)
            .Breadcrumbs(new List<ContentfulReference>()
                        { new() {Title = crumb.Title, Slug = crumb.Title, Sys = new SystemProperties() {Type = "Entry" }},
                        })
            .Build();
        collection.Items = new List<ContentfulContactUsArea> { rawContactUsArea };

        QueryBuilder<ContentfulContactUsArea> builder = new QueryBuilder<ContentfulContactUsArea>().ContentTypeIs("contactUsArea").Include(3);
        _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulContactUsArea>>(q => q.Build().Equals(builder.Build())), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _mockCrumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(crumb);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetContactUsArea());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        ContactUsArea contactUsArea = response.Get<ContactUsArea>();

        contactUsArea.Breadcrumbs.First().Should().Be(crumb);
    }
}
