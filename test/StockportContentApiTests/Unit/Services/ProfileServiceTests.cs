using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Services.Profile;
using Xunit;

namespace StockportContentApiTests.Unit.Services
{
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
        public async Task GetProfileNew_ShouldReturnProfileIfResponseIsOK(){
            // Arrange
            var response = HttpResponse.Successful(new Profile());
            _profileRepository.Setup(_ => _.GetProfile(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await profileService.GetProfile("slug", "stockportgov");

            // Assert
            result.Should().BeOfType(typeof(Profile));
        }

        [Fact]
        public async Task GetProfileNew_ShouldReturnNullIfResponseIsError(){
            // Arrange
            var response = HttpResponse.Failure(HttpStatusCode.InternalServerError, "Error");
            _profileRepository.Setup(_ => _.GetProfile(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await profileService.GetProfile("slug", "stockportgov");

            // Assert
            result.Should().BeNull(null);
        }
    }
}