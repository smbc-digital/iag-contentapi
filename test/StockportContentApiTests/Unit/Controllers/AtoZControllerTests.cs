using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class AtoZControllerTests
{
    private readonly Mock<Func<string, IAtoZRepository>> _mockCreateRepository = new();
    private readonly Mock<IAtoZRepository> _mockRepository = new();
    private readonly AtoZController _controller;

    public AtoZControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new AtoZController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<AtoZ> atoZItems = new()
        {
            new AtoZ("Apple", "apple", "teaser", "article", new List<string>()),
            new AtoZ ("Avocado", "avocado", "teaser", "article", new List < string >())
        };

        _mockRepository
            .Setup(repo => repo.Get(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(atoZItems));

        // Act
        IActionResult result = await _controller.Index("A", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}