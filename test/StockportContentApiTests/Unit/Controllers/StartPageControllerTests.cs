namespace StockportContentApiTests.Unit.Controllers;

public class StartPageControllerTests
{
    private readonly Mock<Func<string, IStartPageRepository>> _createRepository = new();
    private readonly Mock<IStartPageRepository> _repository = new();
    private readonly StartPageController _controller;

    public StartPageControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new StartPageController(new(logger.Object), _createRepository.Object);
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

        _repository
            .Setup(repo => repo.GetStartPage(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(startPage));

        // Act
        IActionResult result = await _controller.GetStartPage("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
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

        _repository
            .Setup(repo => repo.Get("tagId"))
            .ReturnsAsync(HttpResponse.Successful(startPages));

        // Act
        IActionResult result = await _controller.Get("tagId");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}