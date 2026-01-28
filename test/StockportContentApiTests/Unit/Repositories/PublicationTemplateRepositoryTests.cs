namespace StockportContentApiTests.Unit.Repositories;

public class PublicationTemplateRepositoryTests
{
    private readonly PublicationTemplateRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulPublicationPage, PublicationPage>> _publicationPageFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulPublicationSection, PublicationSection>> _publicationSectionFactory = new();
    private readonly Mock<ITimeProvider> _mockTimeProvider = new();
    
    public PublicationTemplateRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();
        
        PublicationPageContentfulFactory contentfulFactory = new(
            _publicationSectionFactory.Object,
            _mockTimeProvider.Object,
            new Mock<IContentfulFactory<ContentfulAlert, Alert>>().Object,
            new Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>>().Object,
            new Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>>().Object
        );

        _mockTimeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2016, 10, 15));

        ContentfulCollection<ContentfulPublicationTemplate> collection = new()
        {
            Items = [new ContentfulPublicationTemplateBuilder()
                .Slug("publication-template-slug")
                .Build()]
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulPublicationTemplate>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(config))
            .Returns(_contentfulClient.Object);

        _repository = new(config,
                        new PublicationTemplateContentfulFactory(_publicationPageFactory.Object,
                            new Mock<IContentfulFactory<ContentfulReference, Crumb>>().Object,
                            _mockTimeProvider.Object,
                            new Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>>().Object),
                        contentfulClientManager.Object);
    }

    [Fact]
    public async Task GetPublicationTemplate_ShouldReturnSuccessfulResponse()
    {
        // Act
        HttpResponse response = await _repository.GetPublicationTemplate("publication-template-slug", "tagId");

        // Assert
        PublicationTemplate result = response.Get<PublicationTemplate>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPublicationTemplate_ShouldReturnNotFoundResponse_IfPublicationTemplateDoesNotExist()
    {
        // Arrange
        ContentfulCollection<ContentfulPublicationTemplate> contentfulCollection = new() { Items = Enumerable.Empty<ContentfulPublicationTemplate>() };
        
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulPublicationTemplate>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);
        
        // Act
        HttpResponse response = await _repository.GetPublicationTemplate("slug", "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No publication template found with slug 'slug' for 'tagId'", response.Error);
    }
}