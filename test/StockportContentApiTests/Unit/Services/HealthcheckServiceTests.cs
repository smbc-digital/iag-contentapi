namespace StockportContentApiTests.Unit.Services;

public class HealthcheckServiceTests
{
    private readonly HealthcheckService _healthcheckService;
    private readonly string _shaPath;
    private readonly string _appVersionPath;
    private readonly Mock<IFileWrapper> _fileWrapperMock;
    private readonly Mock<ICache> _cacheWrapper;
    private const string ExpiryTime = "Expiry";
    private const string Key = "Key";
    private const int NumberOfItems = 4;

    public HealthcheckServiceTests()
    {
        _appVersionPath = "./Unit/version.txt";
        _shaPath = "./Unit/sha.txt";
        _fileWrapperMock = new Mock<IFileWrapper>();
        SetUpFakeFileSystem();
        _cacheWrapper = new Mock<ICache>();

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
    public async void ShouldContainTheAppVersionInTheResponse()
    {
        Healthcheck check = await _healthcheckService.Get();

        check.AppVersion.Should().Be("0.0.3");
    }

    [Fact]
    public async void ShouldContainTheGitShaInTheResponse()
    {
        Healthcheck check = await _healthcheckService.Get();

        check.SHA.Should().Be("sha");
    }

    [Fact]
    public async void ShouldSetAppVersionToDevIfFileNotFound()
    {
        string notFoundVersionPath = "notfound";
        _fileWrapperMock.Setup(x => x.Exists(notFoundVersionPath)).Returns(false);

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(notFoundVersionPath, _shaPath);
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        check.AppVersion.Should().Be("dev");
    }

    [Fact]
    public async void ShouldSetShaToEmptyIfFileNotFound()
    {
        string notFoundShaPath = "notfound";
        _fileWrapperMock.Setup(x => x.Exists(notFoundShaPath)).Returns(false);

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(_appVersionPath, notFoundShaPath);
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        check.SHA.Should().Be(string.Empty);
    }

    [Fact]
    public async void ShouldSetAppVersionToDevIfFileEmpty()
    {
        string newFile = "newFile";
        _fileWrapperMock.Setup(x => x.Exists(newFile)).Returns(true);
        _fileWrapperMock.Setup(x => x.ReadAllLines(newFile)).Returns(new string[] { });

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(newFile, _shaPath);
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        check.AppVersion.Should().Be("dev");
    }

    [Fact]
    public async void ShouldSetAppVersionToDevIfFileHasAnEmptyAString()
    {
        string newFile = "newFile";
        _fileWrapperMock.Setup(x => x.Exists(newFile)).Returns(true);
        _fileWrapperMock.Setup(x => x.ReadAllLines(newFile)).Returns(new[] { string.Empty });

        HealthcheckService healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(newFile, _shaPath);
        Healthcheck check = await healthCheckServiceWithNotFoundVersion.Get();

        check.AppVersion.Should().Be("dev");
    }

    //[Fact]
    //public async void ShouldReturnRedisKeys()
    //{
    //    string newFile = "newFile";
    //    _fileWrapperMock.Setup(x => x.Exists(newFile)).Returns(true);
    //    _fileWrapperMock.Setup(x => x.ReadAllLines(newFile)).Returns(new[] { string.Empty });

    //    var healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(newFile, _shaPath);
    //    var check = await healthCheckServiceWithNotFoundVersion.Get();

    //    var redisData = check.RedisValueData;
    //    redisData[0].Key.Should().Be(Key);
    //    redisData[0].Expiry.Should().Be(ExpiryTime);
    //    redisData[0].NumberOfItems.Should().Be(NumberOfItems);
    //}

}
