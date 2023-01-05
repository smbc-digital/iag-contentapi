using FluentAssertions;
using Moq;
using StockportContentApi.Exceptions;
using StockportContentApi.Utils;
using Xunit;

namespace StockportContentApiTests.Unit.Helpers
{
    public class AuthenticationHelperTests
    {
        private readonly AuthenticationHelper _helper;

        public AuthenticationHelperTests()
        {
            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(_ => _.Now()).Returns(new DateTime(2017, 10, 25));

            _helper = new AuthenticationHelper(timeProvider.Object);
        }

        [Fact]
        public void ExtractAuthenticationDataFromContext_ShouldReturnAuthenticationDataWithCorrectValues()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/v1/stockportgov/articles/test";
            context.Request.Headers.Add("Authorization", "test");
            context.Request.Method = "GET";

            // Act
            var data = _helper.ExtractAuthenticationDataFromContext(context);

            // Assert
            data.Version.Should().Be(1);
            data.AuthenticationKey.Should().Be("test");
            data.BusinessId.Should().Be("stockportgov");
            data.Endpoint.Should().Be("articles");
            data.Verb.Should().Be("GET");
            data.VersionText.Should().Be("v1");
        }

        [Theory]
        [InlineData("article", "articles")]
        [InlineData("group", "groups")]
        [InlineData("payment", "payments")]
        [InlineData("event", "events")]
        [InlineData("topic", "topics")]
        [InlineData("profile", "profiles")]
        [InlineData("start-page", "start pages")]
        [InlineData("organisation", "organisations")]
        public void GetApiEndPoint_ShouldReturnCorrectEndpoint(string requestedEndpoint, string result)
        {
            // Act
            var returnedEndpoint = _helper.GetApiEndPoint(requestedEndpoint);

            // Assert
            returnedEndpoint.Should().Be(result);
        }

        [Fact]
        public void CheckVersionIsProvided_ShouldThrowExceptionIfVersionIsNotProvided()
        {
            // Arrange
            var authData = new AuthenticationData()
            {
                AuthenticationKey = "key",
                BusinessId = "businessid",
                Version = 1,
                Endpoint = "endpoint",
                Verb = "GET",
                VersionText = "incorrectText"
            };

            // Act Assert
            Assert.Throws<AuthorizationException>(() => _helper.CheckVersionIsProvided(authData));
        }

        [Fact]
        public void Invoke_ShouldReturnIfNoApiKeyIsInTheConfig()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/v1/stockportgov/articles/test";
            context.Request.Headers.Add("Authorization", "test");
            context.Request.Method = "GET";
        }
    }
}
