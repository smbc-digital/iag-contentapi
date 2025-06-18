using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class StartPageControllerTests
{
    private readonly Mock<Func<string, IStartPageRepository>> _mockCreateRepository = new();
    private readonly Mock<IStartPageRepository> _mockRepository = new();
    private readonly StartPageController _controller;

    public StartPageControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new StartPageController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetStartPage_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        StartPage startPage = new("title",
                            "slug",
                            "teaser",
                            "summary",
                            "upper body",
                            "form link",
                            "lower body",
                            "background image",
                            "icon",
                            new List<Crumb>(),
                            new List<Alert>(),
                            new List<Alert>());

        _mockRepository
            .Setup(repo => repo.GetStartPage(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(startPage));

        // Act
        IActionResult result = await _controller.GetStartPage("slug", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<StartPage> startPages = new()
        {
            new StartPage("title",
                        "slug",
                        "teaser",
                        "summary",
                        "upper body",
                        "form link",
                        "lower body",
                        "background image",
                        "icon",
                        new List<Crumb>(),
                        new List<Alert>(),
                        new List<Alert>()),
            new StartPage("title2",
                        "slug2",
                        "teaser2",
                        "summary2",
                        "upper body2",
                        "form link2",
                        "lower body2",
                        "background image2",
                        "icon2",
                        new List<Crumb>(),
                        new List<Alert>(),
                        new List<Alert>())
        };

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(startPages));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}