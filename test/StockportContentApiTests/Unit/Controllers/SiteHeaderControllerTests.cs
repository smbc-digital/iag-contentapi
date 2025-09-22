namespace StockportContentApiTests.Unit.Controllers;

public class SiteHeaderControllerTests
{
    private readonly Mock<Func<string, ISiteHeaderRepository>> _createRepository = new();
    private readonly Mock<ISiteHeaderRepository> _repository = new();
    private readonly SiteHeaderController _controller;

    public SiteHeaderControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new SiteHeaderController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetArticle_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        SiteHeader siteHeader = new("title",
                            new List<SubItem>(),
                            "logo");

        _repository
            .Setup(repo => repo.GetSiteHeader())
            .ReturnsAsync(HttpResponse.Successful(siteHeader));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}