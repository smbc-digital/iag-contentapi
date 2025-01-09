using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class SiteHeaderControllerTests
{
    private readonly Mock<Func<string, ISiteHeaderRepository>> _mockCreateRepository = new();
    private readonly Mock<ISiteHeaderRepository> _mockRepository = new();
    private readonly SiteHeaderController _controller;

    public SiteHeaderControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new SiteHeaderController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetArticle_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        SiteHeader siteHeader = new("title",
                            new List<SubItem>(),
                            "logo");

        _mockRepository
            .Setup(repo => repo.GetSiteHeader())
            .ReturnsAsync(HttpResponse.Successful(siteHeader));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}