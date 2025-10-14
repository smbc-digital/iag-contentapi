namespace StockportContentApiTests.Unit.Controllers;

public class ProfileControllerTests
{
    private readonly Mock<Func<string, IProfileRepository>> _createRepository = new();
    private readonly Mock<IProfileRepository> _repository = new();
    private readonly ProfileController _controller;

    public ProfileControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new ProfileController(new(logger.Object), _createRepository.Object);
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

        _repository
            .Setup(repo => repo.GetProfile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(profile));

        // Act
        IActionResult result = await _controller.GetProfile("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
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

        _repository
            .Setup(repo => repo.Get(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(profiles));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}