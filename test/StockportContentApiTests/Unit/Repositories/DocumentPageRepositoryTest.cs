namespace StockportContentApiTests.Unit.Repositories;

public class DocumentPageRepositoryTest
{
    private readonly DocumentPageRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<ICache> _cache;
    private readonly Mock<ITimeProvider> _mockTimeProvider = new();

    public DocumentPageRepositoryTest()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();
        
        _cache = new Mock<ICache>();

        DocumentPageContentfulFactory contentfulFactory = new(
            new Mock<IContentfulFactory<Asset, Document>>().Object,
            new Mock<IContentfulFactory<ContentfulReference, SubItem>>().Object,
            new Mock<IContentfulFactory<ContentfulReference, Crumb>>().Object,
            _mockTimeProvider.Object
        );

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(_ => _.GetClient(config)).Returns(_contentfulClient.Object);
        _repository = new DocumentPageRepository(config, contentfulClientManager.Object, contentfulFactory, _cache.Object);
    }

    [Fact]
    public void GetDocumentPage_ShouldReturnNotFound_If_DocumentDoesNotExist()
    {
        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetDocumentPage("slug"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public void GetDocumentPage_ShouldReturnSuccessfulResponse()
    {
        // Arrange
        ContentfulDocumentPage contentfulDocumentPage = new()
        {
            Slug = "slug",
            AboutTheDocument = "about the document",
            Documents = new List<Asset>(),
            AwsDocuments = "aws documents",
            RequestAnAccessibleFormatContactInformation = "request an accessible format contact information",
            FurtherInformation = "further information",
            RelatedDocuments = new List<ContentfulReference>(),
            DatePublished = new(),
            LastUpdated = new(),
            Breadcrumbs = new List<ContentfulReference>(),
            Sys = new() { UpdatedAt = new DateTime() }
        };

        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2016, 10, 15));
        _cache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s.Equals($"documentPage-slug")), It.IsAny<Func<Task<ContentfulDocumentPage>>>())).ReturnsAsync(contentfulDocumentPage);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetDocumentPage("slug"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}