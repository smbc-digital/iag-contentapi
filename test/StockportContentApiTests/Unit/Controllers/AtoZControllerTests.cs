namespace StockportContentApiTests.Unit.Controllers;

public class AtoZControllerTests
{
    private readonly Mock<Func<string, IAtoZRepository>> _createRepository = new();
    private readonly Mock<IAtoZRepository> _repository = new();
    private readonly AtoZController _controller;

    public AtoZControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new AtoZController(new(logger.Object), _createRepository.Object);
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

        _repository
            .Setup(repo => repo.Get(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(atoZItems));

        // Act
        IActionResult result = await _controller.Index("A", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WhenRepositoryWithNoLetterReturnsSuccessfulResponse()
    {
        // Arrange
        List<AtoZ> atoZItems = new()
        {
            new AtoZ("Apple", "apple", "teaser", "article", new List<string>()),
            new AtoZ ("Avocado", "avocado", "teaser", "topic", new List < string >())
        };

        _repository
            .Setup(repo => repo.Get(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(atoZItems));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}