using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class FooterControllerTests
{
    private readonly Mock<Func<string, IFooterRepository>> _mockCreateRepository = new();
    private readonly Mock<IFooterRepository> _mockRepository = new();
    private readonly FooterController _controller;

    public FooterControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new FooterController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetFooter_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Footer footer = new("footer title",
                            "footer-slug",
                            new List<SubItem>(),
                            new List<SocialMediaLink>(),
                            "footer content one",
                            "footer content two",
                            "footer content 3");

        _mockRepository
            .Setup(repo => repo.GetFooter())
            .ReturnsAsync(HttpResponse.Successful(footer));

        // Act
        IActionResult result = await _controller.GetFooter("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}