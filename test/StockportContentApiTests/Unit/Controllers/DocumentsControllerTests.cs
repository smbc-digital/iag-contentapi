namespace StockportContentApiTests.Unit.Controllers;

public class DocumentsControllerTests
{
    private readonly Mock<IDocumentService> _service = new();
    private readonly DocumentsController _controller;

    public DocumentsControllerTests()
    {
        Mock<ILogger<DocumentsController>> logger = new();

        _controller = new DocumentsController(_service.Object, logger.Object);
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

        _service
            .Setup(repo => repo.GetSecureDocumentByAssetId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(document));

        // Act
        IActionResult result = await _controller.GetSecureDocument("test-business", "group-slug", "asset-id");

        // Assert
        _service.Verify(service => service.GetSecureDocumentByAssetId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}