namespace StockportContentApiTests.Unit.Controllers;

public class FooterControllerTests
{
    private readonly Mock<Func<string, IFooterRepository>> _categoryRepository = new();
    private readonly Mock<IFooterRepository> _repository = new();
    private readonly FooterController _controller;

    public FooterControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _categoryRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new FooterController(new(logger.Object), _categoryRepository.Object);
    }

    [Fact]
    public async Task GetFooter_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Footer footer = new("title",
                            "slug",
                            new List<SubItem>(),
                            new List<SocialMediaLink>(),
                            "footer content 1",
                            "footer content 2",
                            "footer content 3");

        _repository
            .Setup(repo => repo.GetFooter("tagId"))
            .ReturnsAsync(HttpResponse.Successful(footer));

        // Act
        IActionResult result = await _controller.GetFooter("tagId");

        // Assert
        _categoryRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}