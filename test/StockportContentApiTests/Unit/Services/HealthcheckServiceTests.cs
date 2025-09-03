namespace StockportContentApiTests.Unit.Services;

public class HealthcheckServiceTests
{
    private const string ExpiryTime = "Expiry";
    private const string Key = "Key";
    private const int NumberOfItems = 4;
    private readonly string _appVersionPath;
    private readonly Mock<ICache> _cacheWrapper = new();
    private readonly Mock<IFileWrapper> _fileWrapperMock = new();
    private readonly HealthcheckService _healthcheckService;
    private readonly string _shaPath;

    public HealthcheckServiceTests()
    {
        _appVersionPath = "./Unit/version.txt";
        _shaPath = "./Unit/sha.txt";
        SetUpFakeFileSystem();

        _healthcheckService = CreateHealthcheckService(_appVersionPath, _shaPath);
    }

    private void SetUpFakeFileSystem()
    {
        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.Exists(_appVersionPath))
            .Returns(true);

        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.ReadAllLines(_appVersionPath))
            .Returns(["0.0.3"]);

        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.Exists(_shaPath))
            .Returns(true);

        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.ReadAllLines(_shaPath))
            .Returns(["sha"]);
    }

    private HealthcheckService CreateHealthcheckService(string appVersionPath, string shaPath) =>
        new(appVersionPath, shaPath, _fileWrapperMock.Object, "local");

    [Fact]
    public async Task ShouldContainTheAppVersionInTheResponse()
    {
        // Act
        Healthcheck check = await _healthcheckService.Get();

        // Assert
        Assert.Equal("0.0.3", check.AppVersion);
    }

    [Fact]
    public async Task ShouldContainTheGitShaInTheResponse()
    {
        // Act
        Healthcheck check = await _healthcheckService.Get();

        // Assert
        Assert.Equal("sha", check.SHA);
    }

    [Fact]
    public async Task ShouldSetAppVersionToDevIfFileNotFound()
    {
        // Arrange
        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.Exists("notfound"))
            .Returns(false);

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService("notfound", _shaPath);

        // Act
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        // Assert
        Assert.Equal("dev", check.AppVersion);
    }

    [Fact]
    public async Task ShouldSetShaToEmptyIfFileNotFound()
    {
        // Arrange
        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.Exists("notfound"))
            .Returns(false);

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(_appVersionPath, "notfound");

        // Act
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        // Assert
        Assert.Empty(check.SHA);
    }

    [Fact]
    public async Task ShouldSetAppVersionToDevIfFileEmpty()
    {
        // Arrange
        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.Exists("newFile"))
            .Returns(true);

        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.ReadAllLines("newFile"))
            .Returns(Array.Empty<string>());

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService("newFile", _shaPath);

        // Act
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        // Assert
        Assert.Equal("dev", check.AppVersion);
    }

    [Fact]
    public async Task ShouldSetAppVersionToDevIfFileHasAnEmptyAString()
    {
        // Arrange
        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.Exists("newFile"))
            .Returns(true);

        _fileWrapperMock
            .Setup(fileWrapperMock => fileWrapperMock.ReadAllLines("newFile"))
            .Returns([string.Empty]);

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService("newFile", _shaPath);

        // Act
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        // Assert
        Assert.Equal("dev", check.AppVersion);
    }
}