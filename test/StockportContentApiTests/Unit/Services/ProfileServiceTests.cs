namespace StockportContentApiTests.Unit.Services;

public class ProfileServiceTests
{
    private readonly ProfileService profileService;
    private readonly Mock<Func<string, ContentfulConfig>> _createConfig;
    private readonly Mock<Func<ContentfulConfig, IProfileRepository>> _createRepository;
    private readonly Mock<IProfileRepository> _profileRepository;
    public ProfileServiceTests()
    {
        _profileRepository = new Mock<IProfileRepository>();
        _createRepository = new Mock<Func<ContentfulConfig, IProfileRepository>>();
        _createConfig = new Mock<Func<string, ContentfulConfig>>();
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
        result.Should().BeOfType(typeof(Profile));
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
        result.Should().BeNull(null);
    }
}