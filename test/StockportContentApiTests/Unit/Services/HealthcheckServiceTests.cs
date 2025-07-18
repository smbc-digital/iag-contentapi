namespace StockportContentApiTests.Unit.Services;

public class HealthcheckServiceTests
{
    private const string ExpiryTime = "Expiry";
    private const string Key = "Key";
    private const int NumberOfItems = 4;
    private readonly string _appVersionPath;
    private readonly Mock<ICache> _cacheWrapper;
    private readonly Mock<IFileWrapper> _fileWrapperMock;
    private readonly HealthcheckService _healthcheckService;
    private readonly string _shaPath;

    public HealthcheckServiceTests()
    {
        _appVersionPath = "./Unit/version.txt";
        _shaPath = "./Unit/sha.txt";
        _fileWrapperMock = new();
        SetUpFakeFileSystem();
        _cacheWrapper = new();

        _healthcheckService = CreateHealthcheckService(_appVersionPath, _shaPath);
    }

    private void SetUpFakeFileSystem()
    {
        _fileWrapperMock.Setup(x => x.Exists(_appVersionPath)).Returns(true);
        _fileWrapperMock.Setup(x => x.ReadAllLines(_appVersionPath)).Returns(new[] { "0.0.3" });
        _fileWrapperMock.Setup(x => x.Exists(_shaPath)).Returns(true);
        _fileWrapperMock.Setup(x => x.ReadAllLines(_shaPath)).Returns(new[] { "sha" });
    }

    private HealthcheckService CreateHealthcheckService(string appVersionPath, string shaPath) =>
        new(appVersionPath, shaPath, _fileWrapperMock.Object, "local");


    [Fact]
    public async Task ShouldContainTheAppVersionInTheResponse()
    {
        Healthcheck check = await _healthcheckService.Get();

        check.AppVersion.Should().Be("0.0.3");
    }

    [Fact]
    public async Task ShouldContainTheGitShaInTheResponse()
    {
        Healthcheck check = await _healthcheckService.Get();

        check.SHA.Should().Be("sha");
    }

    [Fact]
    public async Task ShouldSetAppVersionToDevIfFileNotFound()
    {
        string notFoundVersionPath = "notfound";
        _fileWrapperMock.Setup(x => x.Exists(notFoundVersionPath)).Returns(false);

        HealthcheckService healthCheckServiceWithNotFoundVersion =
            CreateHealthcheckService(notFoundVersionPath, _shaPath);
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        check.AppVersion.Should().Be("dev");
    }

    [Fact]
    public async Task ShouldSetShaToEmptyIfFileNotFound()
    {
        string notFoundShaPath = "notfound";
        _fileWrapperMock.Setup(x => x.Exists(notFoundShaPath)).Returns(false);

        HealthcheckService healthCheckServiceWithNotFoundVersion =
            CreateHealthcheckService(_appVersionPath, notFoundShaPath);
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        check.SHA.Should().Be(string.Empty);
    }

    [Fact]
    public async Task ShouldSetAppVersionToDevIfFileEmpty()
    {
        string newFile = "newFile";
        _fileWrapperMock.Setup(x => x.Exists(newFile)).Returns(true);
        _fileWrapperMock.Setup(x => x.ReadAllLines(newFile)).Returns(new string[] { });

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(newFile, _shaPath);
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        check.AppVersion.Should().Be("dev");
    }

    [Fact]
    public async Task ShouldSetAppVersionToDevIfFileHasAnEmptyAString()
    {
        string newFile = "newFile";
        _fileWrapperMock.Setup(x => x.Exists(newFile)).Returns(true);
        _fileWrapperMock.Setup(x => x.ReadAllLines(newFile)).Returns(new[] { string.Empty });

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(newFile, _shaPath);
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        check.AppVersion.Should().Be("dev");
    }
}