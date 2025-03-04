using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class ProfileControllerTests
{
    private readonly Mock<Func<string, IProfileRepository>> _mockCreateRepository = new();
    private readonly Mock<IProfileRepository> _mockRepository = new();
    private readonly ProfileController _controller;

    public ProfileControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new ProfileController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetProfile_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Profile profile = new()
        {
            Title = "title",
            Slug = "slug"
        };

        _mockRepository
            .Setup(repo => repo.GetProfile(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(profile));

        // Act
        IActionResult result = await _controller.GetProfile("slug", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<Profile> profiles = new()
        {
            new Profile()
            {
                Title = "profile1",
                Slug = "profile-one"
            },
            new Profile()
            {
                Title = "profile2",
                Slug = "profile-two"
            },
        };

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(profiles));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}