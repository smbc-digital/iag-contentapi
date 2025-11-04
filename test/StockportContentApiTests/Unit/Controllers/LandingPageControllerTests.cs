namespace StockportContentApiTests.Unit.Controllers;

public class LandingPageControllerTests
{
    private readonly Mock<Func<string, string, ILandingPageRepository>> _createRepository = new();
    private readonly Mock<ILandingPageRepository> _repository = new();
    private readonly LandingPageController _controller;

    public LandingPageControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new LandingPageController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetLandingPage_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        LandingPage landingPage = new()
        {
            Slug = "slug",
            Title = "title"
        };

        _repository
            .Setup(repo => repo.GetLandingPage(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(landingPage));

        // Act
        IActionResult result = await _controller.GetLandingPage("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}