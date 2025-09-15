namespace StockportContentApiTests.Unit.Controllers;

public class DocumentPageControllerTests
{
    private readonly Mock<Func<string, IDocumentPageRepository>> _createRepository = new();
    private readonly Mock<IDocumentPageRepository> _repository = new();
    private readonly DocumentPageController _controller;

    public DocumentPageControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new DocumentPageController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetDocumentPage_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        DocumentPage documentPage = new()
        {
            Title = "document page",
            Slug = "document-page"
        };

        _repository
            .Setup(repo => repo.GetDocumentPage(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(documentPage));

        // Act
        IActionResult result = await _controller.GetDocumentPage("document-page", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}