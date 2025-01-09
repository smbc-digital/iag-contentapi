using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class NewsControllerTests
{
    private readonly Mock<Func<string, INewsRepository>> _mockCreateRepository = new();
    private readonly Mock<INewsRepository> _mockRepository = new();
    private readonly NewsController _controller;

    public NewsControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new NewsController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task LatestNews_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Newsroom newsroom = new(new List<Alert>(),
                                false,
                                "email alerts topic id");

        _mockRepository
            .Setup(repo => repo.GetNewsByLimit(It.IsAny<int>()))
            .ReturnsAsync(HttpResponse.Successful(newsroom));

        // Act
        IActionResult result = await _controller.LatestNews("test-business", 5);

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Detail_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Newsroom newsroom = new(new List<Alert>(),
                                false,
                                "email alerts topic id");

        _mockRepository
            .Setup(repo => repo.GetNews(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(newsroom));

        // Act
        IActionResult result = await _controller.Detail("slug", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}