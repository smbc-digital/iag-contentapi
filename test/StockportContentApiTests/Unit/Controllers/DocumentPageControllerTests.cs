using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class DocumentPageControllerTests
{
    private readonly Mock<Func<string, IDocumentPageRepository>> _mockCreateRepository = new();
    private readonly Mock<IDocumentPageRepository> _mockRepository = new();
    private readonly DocumentPageController _controller;

    public DocumentPageControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new DocumentPageController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetDocumentPage_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        DocumentPage expectedDocumentPage = new()
        {
            Title = "document page",
            Slug = "document-page"
        };

        _mockRepository
            .Setup(repo => repo.GetDocumentPage(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(new DocumentPage()));

        // Act
        IActionResult result = await _controller.GetDocumentPage("document-page", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}