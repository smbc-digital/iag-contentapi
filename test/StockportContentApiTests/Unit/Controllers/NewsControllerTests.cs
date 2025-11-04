namespace StockportContentApiTests.Unit.Controllers;

public class NewsControllerTests
{
    private readonly Mock<Func<string, INewsRepository>> _createRepository = new();
    private readonly Mock<INewsRepository> _repository = new();
    private readonly NewsController _controller;

    public NewsControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new NewsController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task LatestNews_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Newsroom newsroom = new(new List<Alert>(),
                                false,
                                "email alerts topic id",
                                null);

        _repository
            .Setup(repo => repo.GetNewsByLimit(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(newsroom));

        // Act
        IActionResult result = await _controller.LatestNews("test-business", 5);

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Detail_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Newsroom newsroom = new(new List<Alert>(),
                                false,
                                "email alerts topic id",
                                null);

        _repository
            .Setup(repo => repo.GetNews(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(newsroom));

        // Act
        IActionResult result = await _controller.Detail("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Newsroom newsroom = new(new List<Alert>(),
                                false,
                                "email alerts topic id",
                                null);

        _repository
            .Setup(repo => repo.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(newsroom));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ArchivedNews_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Newsroom newsroom = new(new List<Alert>(),
                                false,
                                "email alerts topic id",
                                null);

        _repository
            .Setup(repo => repo.GetArchivedNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(newsroom));

        // Act
        IActionResult result = await _controller.ArchivedNews("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}