using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class DocumentsControllerTests
{
    private readonly Mock<IDocumentService> _mockService = new();
    private readonly DocumentsController _controller;

    public DocumentsControllerTests()
    {
        Mock<ILogger<DocumentsController>> mockLogger = new();

        _controller = new DocumentsController(_mockService.Object, mockLogger.Object);
    }

    [Fact]
    public async Task GetDocumentPage_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Document document = new()
        {
            Title = "document page",
            Size = 123,
            Url = "url",
            LastUpdated = new DateTime(),
            FileName = "file name",
            AssetId = "asset id",
            MediaType = "media type"
        };

        _mockService
            .Setup(repo => repo.GetSecureDocumentByAssetId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(document));

        // Act
        IActionResult result = await _controller.GetSecureDocument("test-business", "group-slug", "asset-id");

        // Assert
        _mockService.Verify(service => service.GetSecureDocumentByAssetId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}