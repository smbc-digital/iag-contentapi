using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class LandingPageControllerTests
{
    private readonly Mock<Func<string, string, ILandingPageRepository>> _mockCreateRepository = new();
    private readonly Mock<ILandingPageRepository> _mockRepository = new();
    private readonly LandingPageController _controller;

    public LandingPageControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new LandingPageController(new(mockLogger.Object), _mockCreateRepository.Object);
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

        _mockRepository
            .Setup(repo => repo.GetLandingPage(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(landingPage));

        // Act
        IActionResult result = await _controller.GetLandingPage("slug", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}