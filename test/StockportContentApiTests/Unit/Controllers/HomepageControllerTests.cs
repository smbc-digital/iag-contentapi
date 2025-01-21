using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class HomepageControllerTests
{
    private readonly Mock<Func<string, IHomepageRepository>> _mockCreateRepository = new();
    private readonly Mock<IHomepageRepository> _mockRepository = new();
    private readonly HomepageController _controller;

    public HomepageControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new HomepageController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Homepage homepage = new(new List<string>(),
                                "featured tasks heading",
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
                                null,
                                "image overlay text");

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(homepage));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}