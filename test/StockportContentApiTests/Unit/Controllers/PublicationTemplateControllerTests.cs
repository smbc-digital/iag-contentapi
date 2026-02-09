namespace StockportContentApiTests.Unit.Controllers;

public class PublicationTemplateControllerTests
{
    private readonly Mock<Func<string, IPublicationTemplateRepository>> _createRepository = new();
    private readonly Mock<IPublicationTemplateRepository> _repository = new();
    private readonly PublicationTemplateController _controller;

    public PublicationTemplateControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new PublicationTemplateController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetPublicationTemplate_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        PublicationTemplate publicationTemplate = new()
        {
            Title = "Publication template title",
            Slug = "publication-template-slug",
            MetaDescription = "publication template meta description",
            HeaderImage = new MediaAsset
            {
                Url = "publication-template-image.jpg",
            }
        };

        _repository
            .Setup(repo => repo.GetPublicationTemplate(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(publicationTemplate));

        // Act
        IActionResult result = await _controller.GetPublicationTemplate("publication-template", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}