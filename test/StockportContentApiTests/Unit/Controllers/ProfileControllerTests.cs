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
    public async Task Get_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<Profile> profiles = new()
        {
            new()
            {
                Title = "Profile one",
                Slug = "profile-one"
            },
            new()
            {
                Title = "Profile two",
                Slug = "profile-two"
            }
        };

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(profiles));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetProfile_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Profile profile = new()
        {
            Title = "Test profile",
            Slug = "profile"
        };

        _mockRepository
            .Setup(repo => repo.GetProfile(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(profile));

        // Act
        IActionResult result = await _controller.GetProfile("profile", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}