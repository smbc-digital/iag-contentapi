namespace StockportContentApiTests.Unit.Services;

public class ProfileServiceTests
{
    private readonly ProfileService profileService;
    private readonly Mock<Func<string, ContentfulConfig>> _createConfig = new();
    private readonly Mock<Func<ContentfulConfig, IProfileRepository>> _createRepository = new();
    private readonly Mock<IProfileRepository> _profileRepository = new();
    public ProfileServiceTests()
    {
        _createRepository.Setup(_ => _(It.IsAny<ContentfulConfig>())).Returns(_profileRepository.Object);
        profileService = new ProfileService(_createConfig.Object, _createRepository.Object);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnProfileIfResponseIsOK()
    {
        // Arrange
        var response = HttpResponse.Successful(new Profile());
        _profileRepository.Setup(_ => _.GetProfile(It.IsAny<string>())).ReturnsAsync(response);

        // Act
        var result = await profileService.GetProfile("slug", "stockportgov");

        // Assert
        Assert.IsType<Profile>(result);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnNullIfResponseIsError()
    {
        // Arrange
        var response = HttpResponse.Failure(HttpStatusCode.InternalServerError, "Error");
        _profileRepository.Setup(_ => _.GetProfile(It.IsAny<string>())).ReturnsAsync(response);

        // Act
        var result = await profileService.GetProfile("slug", "stockportgov");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Get_ShouldReturnProfileIfResponseIsOK()
    {
        // Arrange
        var response = HttpResponse.Successful(new List<Profile>());
        _profileRepository.Setup(_ => _.Get()).ReturnsAsync(response);

        // Act
        var result = await profileService.GetProfiles("stockportgov");

        // Assert
        Assert.IsType<List<Profile>>(result);
    }

    [Fact]
    public async Task Get_ShouldReturnNullIfResponseIsError()
    {
        // Arrange
        var response = HttpResponse.Failure(HttpStatusCode.InternalServerError, "Error");
        _profileRepository.Setup(_ => _.Get()).ReturnsAsync(response);

        // Act
        var result = await profileService.GetProfiles("stockportgov");

        // Assert
        Assert.Null(result);
    }
}