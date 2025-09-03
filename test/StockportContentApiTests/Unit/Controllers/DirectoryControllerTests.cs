using Directory = StockportContentApi.Models.Directory;

namespace StockportContentApiTests.Unit.Controllers;

public class DirectoryControllerTests
{
    private readonly Mock<Func<string, IDirectoryRepository>> _createRepository = new();
    private readonly Mock<IDirectoryRepository> _repository = new();
    private readonly DirectoryController _controller;

    public DirectoryControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new DirectoryController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetDirectories_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Directory directory = new()
        {
            Title = "Directory",
            Slug = "directory",
            ContentfulId = "contentful-id",
            Teaser = "teaser",
            MetaDescription = "meta-description",
            BackgroundImage = "background-image",
            Body = "body",
        };

        _repository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(directory));

        // Act
        IActionResult result = await _controller.GetDirectories("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetDirectory_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Directory directory = new()
        {
            Title = "Directory",
            Slug = "directory",
            ContentfulId = "contentful-id"
        };

        _repository
            .Setup(repo => repo.Get(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(directory));

        // Act
        IActionResult result = await _controller.GetDirectory("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetDirectoryEntry_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Directory directory = new()
        {
            Title = "Directory",
            Slug = "directory"
        };

        _repository
            .Setup(repo => repo.GetEntry(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(directory));

        // Act
        IActionResult result = await _controller.GetDirectoryEntry("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}