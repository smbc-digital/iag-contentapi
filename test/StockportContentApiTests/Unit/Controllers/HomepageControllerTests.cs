namespace StockportContentApiTests.Unit.Controllers;

public class HomepageControllerTests
{
    private readonly Mock<Func<string, IHomepageRepository>> _createRepository = new();
    private readonly Mock<IHomepageRepository> _repository = new();
    private readonly HomepageController _controller;

    public HomepageControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new HomepageController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Homepage homepage = new("featured tasks heading",
                                "featured tasks subheading",
                                new List<SubItem>(),
                                new List<SubItem>(),
                                new List<Alert>(),
                                new List<CarouselContent>(),
                                "background image",
                                "foreground image",
                                "foreground image location",
                                "foreground image link",
                                "foreground image alt",
                                "free text",
                                "title",
                                "event category",
                                "meta description",
                                null,
                                null,
                                null,
                                null);

        _repository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(homepage));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}