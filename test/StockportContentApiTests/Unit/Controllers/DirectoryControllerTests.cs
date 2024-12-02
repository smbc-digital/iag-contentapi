using StockportContentApi.Controllers;
using Directory = StockportContentApi.Models.Directory;

namespace StockportContentApiTests.Unit.Controllers;

public class DirectoryControllerTests
{
    private readonly Mock<Func<string, IDirectoryRepository>> _mockCreateRepository = new();
    private readonly Mock<IDirectoryRepository> _mockRepository = new();
    private readonly DirectoryController _controller;

    public DirectoryControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new DirectoryController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetDirectories_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<Directory> directories = new()
        {
            new()
            {
                Title = "Test directory 1",
                Slug = "test-directory-one",
                Entries = new List<DirectoryEntry>()
                {
                    new()
                    {
                        Name = "Test directory entry 1",
                        Slug = "test-directory-entry-one"
                    }
                }
            },
            new()
            {
                Title = "Test directory 2",
                Slug = "test-directory-two"
            }
        };

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(directories));

        // Act
        IActionResult result = await _controller.GetDirectories("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetDirectory_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Directory directory = new()
        {
            Title = "Test directory",
            Slug = "directory"
        };

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(directory));

        // Act
        IActionResult result = await _controller.GetDirectory("directory", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetDirectoryEntry_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        DirectoryEntry directoryEntry = new()
        {
            Name = "Directory entry",
            Slug = "directory-entry"
        };

        _mockRepository
            .Setup(repo => repo.GetEntry("directory-entry"))
            .ReturnsAsync(HttpResponse.Successful(directoryEntry));

        // Act
        IActionResult result = await _controller.GetDirectoryEntry("directory-entry", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}