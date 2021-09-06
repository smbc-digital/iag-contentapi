using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using StockportContentApi.Exceptions;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Helpers
{
    public class AuthenticationHelperTests
    {
        private readonly AuthenticationHelper _helper;
        private readonly Mock<IApiKeyRepository> _repository;

        public AuthenticationHelperTests()
        {
            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(_ => _.Now()).Returns(new DateTime(2017, 10, 25));
            _repository = new Mock<IApiKeyRepository>();

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

        [Fact]
        public void GetValidKey_ShouldThrowExceptionIfNoKeysAreReturned()
        {
            // Arrange
            const string authenticationKey = "test";
            const string businessId = "stockportgov";
            const string endpoint = "articles";
            const int version = 1;
            const string verb = "GET";
            _repository.Setup(_ => _.Get()).ReturnsAsync(new List<ApiKey>());

            // Act Assert
            Assert.ThrowsAsync<AuthorizationException>(async () =>
            {
                    await _helper.GetValidKey(_repository.Object, authenticationKey, businessId, endpoint, version, verb);
            });
        }

        [Fact]
        public async Task GetValidKey_ShouldThrowExceptionWhenKeyDoesNotMatchAsync()
        {
            // Arrange
            var apiKey = new ApiKey("name", "test-fail", "email", DateTime.MinValue, DateTime.MaxValue, new List<string>() { "articles" }, 1, true, new List<string>() { "GET" });
            _repository.Setup(_ => _.Get()).ReturnsAsync(new List<ApiKey>() { apiKey });

            // Act Assert
            await Assert.ThrowsAsync<AuthorizationException>(() => _helper.GetValidKey(_repository.Object, "Bearer test", "stockportgov", "articles", 1, "GET"));

        }

        [Fact]
        public async void GetValidKey_ShouldThrowExceptionWhenKeyIsNotActive()
        {
            // Arrange
            var apiKey = new ApiKey("name", "test", "email", new DateTime(2020, 10, 15), DateTime.MaxValue, new List<string>() { "articles" }, 1, true, new List<string>() { "GET" });
            _repository.Setup(_ => _.Get()).ReturnsAsync(new List<ApiKey>() { apiKey });

            // Act Assert
            await Assert.ThrowsAsync<AuthorizationException>(() => _helper.GetValidKey(_repository.Object, "Bearer test", "stockportgov", "articles", 1, "GET"));
        }

        [Fact]
        public async void GetValidKey_ShouldThrowExceptionWhenKeyDoesNotAllowEndpointRequested()
        {
            // Arrange
            var apiKey = new ApiKey("name", "test", "email", DateTime.MinValue, DateTime.MaxValue, new List<string>() { "articles" }, 1, true, new List<string>() { "GET" });
            _repository.Setup(_ => _.Get()).ReturnsAsync(new List<ApiKey>() { apiKey });

            // Act Assert
            await Assert.ThrowsAsync<AuthorizationException>(() => _helper.GetValidKey(_repository.Object, "Bearer test", "stockportgov", "topics", 1, "GET"));
        }

        [Fact]
        public async void GetValidKey_ShouldThrowExceptionWhenKeyDoesNotAllowVersionRequested()
        {
            // Arrange
            var apiKey = new ApiKey("name", "test", "email", DateTime.MinValue, DateTime.MaxValue, new List<string>() { "articles" }, 1, true, new List<string>() { "GET" });
            _repository.Setup(_ => _.Get()).ReturnsAsync(new List<ApiKey>() { apiKey });

            // Act Assert
            await Assert.ThrowsAsync<AuthorizationException>(() => _helper.GetValidKey(_repository.Object, "Bearer test", "stockportgov", "articles", 2, "GET"));
        }

        [Fact]
        public async void GetValidKey_ShouldThrowExceptionWhenKeyDoesNotAllowVerbRequested()
        {
            // Arrange
            var apiKey = new ApiKey("name", "test", "email", DateTime.MinValue, DateTime.MaxValue, new List<string>() { "articles" }, 1, true, new List<string>() { "GET" });
            _repository.Setup(_ => _.Get()).ReturnsAsync(new List<ApiKey>() { apiKey });

            // Act Assert
            await Assert.ThrowsAsync<AuthorizationException>(() => _helper.GetValidKey(_repository.Object, "Bearer test", "stockportgov", "articles", 1, "POST"));
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HandleSensitiveData_ShouldReturnCorrectSensitivitySetting(bool canViewSensitiveData)
        {
            // Arrange
            var apiKey = new ApiKey("name", "test", "email", DateTime.MinValue, DateTime.MaxValue, new List<string>() { "articles" }, 1, canViewSensitiveData, new List<string>() { "GET" });
            var context = new DefaultHttpContext();
            context.Request.Path = "/v1/stockportgov/articles/test";
            context.Request.Headers.Add("Authorization", "test");
            context.Request.Method = "GET";

            // Act
            _helper.HandleSensitiveData(context, apiKey);

            // Assert
            context.Request.Headers.Keys.Should().Contain("cannotViewSensitive");
            context.Request.Headers["cannotViewSensitive"].ToString().ToLower().Should()
                .Be((!canViewSensitiveData).ToString().ToLower());
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
