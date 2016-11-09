using Moq;
using StockportContentApi.Services;
using StockportContentApi.Utils;
using Xunit;
using FluentAssertions;

namespace StockportContentApiTests.Unit.Services
{
    public class HealthcheckServiceTest
    {
        private readonly HealthcheckService _healthcheckService;
        private readonly string _shaPath;
        private readonly string _appVersionPath;
        private readonly Mock<IFileWrapper> _fileWrapperMock;

        public HealthcheckServiceTest()
        {
            _appVersionPath = "./Unit/version.txt";
            _shaPath = "./Unit/sha.txt";
            _fileWrapperMock = new Mock<IFileWrapper>();
            SetUpFakeFileSystem();
            _healthcheckService = CreateHealthcheckService(_appVersionPath, _shaPath);
        }

        private void SetUpFakeFileSystem()
        {
            _fileWrapperMock.Setup(x => x.Exists(_appVersionPath)).Returns(true);
            _fileWrapperMock.Setup(x => x.ReadAllLines(_appVersionPath)).Returns(new[] { "0.0.3" });
            _fileWrapperMock.Setup(x => x.Exists(_shaPath)).Returns(true);
            _fileWrapperMock.Setup(x => x.ReadAllLines(_shaPath)).Returns(new[] { "sha" });
        }

        private HealthcheckService CreateHealthcheckService(string appVersionPath, string shaPath)
        {
            return new HealthcheckService(appVersionPath, shaPath, _fileWrapperMock.Object);
        }
        [Fact]
        public void ShouldContainTheAppVersionInTheResponse()
        {
            var check = _healthcheckService.Get();

            check.AppVersion.Should().Be("0.0.3");
        }

        [Fact]
        public void ShouldContainTheGitShaInTheResponse()
        {
            var check = _healthcheckService.Get();

            check.SHA.Should().Be("sha");
        }

        [Fact]
        public void ShouldSetAppVersionToDevIfFileNotFound()
        {
            var notFoundVersionPath = "notfound";
            _fileWrapperMock.Setup(x => x.Exists(notFoundVersionPath)).Returns(false);

            var healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(notFoundVersionPath, _shaPath);
            var check = healthCheckServiceWithNotFoundVersion.Get();

            check.AppVersion.Should().Be("dev");
        }

        [Fact]
        public void ShouldSetShaToEmptyIfFileNotFound()
        {
            var notFoundShaPath = "notfound";
            _fileWrapperMock.Setup(x => x.Exists(notFoundShaPath)).Returns(false);

            var healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(_appVersionPath, notFoundShaPath);
            var check = healthCheckServiceWithNotFoundVersion.Get();

            check.SHA.Should().Be("");
        }

        [Fact]
        public void ShouldSetAppVersionToDevIfFileEmpty()
        {
            string newFile = "newFile";
            _fileWrapperMock.Setup(x => x.Exists(newFile)).Returns(true);
            _fileWrapperMock.Setup(x => x.ReadAllLines(newFile)).Returns(new string[] { });

            var healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(newFile, _shaPath);
            var check = healthCheckServiceWithNotFoundVersion.Get();

            check.AppVersion.Should().Be("dev");
        }

        [Fact]
        public void ShouldSetAppVersionToDevIfFileHasAnEmptyAString()
        {
            string newFile = "newFile";
            _fileWrapperMock.Setup(x => x.Exists(newFile)).Returns(true);
            _fileWrapperMock.Setup(x => x.ReadAllLines(newFile)).Returns(new [] { "" });

            var healthCheckServiceWithNotFoundVersion = CreateHealthcheckService(newFile, _shaPath);
            var check = healthCheckServiceWithNotFoundVersion.Get();

            check.AppVersion.Should().Be("dev");
        }
    }
}
